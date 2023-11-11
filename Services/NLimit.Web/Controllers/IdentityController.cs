using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using NLimit.Web.Models;
using System.Text.Encodings.Web;
using System.Text;
using NLimit.Web.AppServices;
using Microsoft.AspNetCore.Authorization;
using Data.NLimit.Common.EntitiesModels.SqlServer;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Net;
using Highlight;
using Data.NLimit.Common.DataContext.SqlServer;
using System.Composition;

namespace NLimit.Web.Controllers;

public class IdentityController : Controller
{
    private readonly IHttpClientFactory clientFactory;

    private readonly SignInManager<IdentityUser> signInManager;

    private readonly NLimitContext db;

    private readonly ILogger<LoginViewModel> loginLogger;

    private readonly UserManager<IdentityUser> userManager;
    private readonly IUserStore<IdentityUser> userStore;
    private readonly IUserEmailStore<IdentityUser> emailStore;
    private readonly ILogger<RegisterModel> regLogger;
    private readonly IEmailSender emailSender;

    public IdentityController(SignInManager<IdentityUser> signInManager, ILogger<LoginViewModel> loginLogger,
        UserManager<IdentityUser> userManager,
        IUserStore<IdentityUser> userStore,
        ILogger<RegisterModel> regLogger,
        IEmailSender emailSender,
        NLimitContext injectedContext,
        IHttpClientFactory httpClientFactory)
    {
        this.signInManager = signInManager;
        this.loginLogger = loginLogger;
        this.userManager = userManager;
        this.userStore = userStore;
        this.regLogger = regLogger;
        this.emailSender = emailSender;
        emailStore = GetEmailStore();
        db = injectedContext;
        clientFactory = httpClientFactory;
    }

    [HttpGet]
    public async Task<IActionResult> Login(AuthStatus? status, string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        return View(new LoginViewModel
        {
            ReturnUrl = returnUrl,
            AuthStatus = status ?? AuthStatus.None
        });
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        model.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        if (!(string.IsNullOrEmpty(model.Email) || string.IsNullOrWhiteSpace(model.Email) ||
            string.IsNullOrEmpty(model.Password) || string.IsNullOrWhiteSpace(model.Password)))
        {
            // ищем пользователя
            var user = await userManager.FindByNameAsync(model.Email);
            if (user is not null)
            {
                // проверяем, подтвержден ли email
                if (!await userManager.IsEmailConfirmedAsync(user))
                {
                    return RedirectToAction("Login", new { status = AuthStatus.EmailNotConfirmed });
                }

                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                // если успешно авторизовались
                if (result.Succeeded)
                {
                    // проверяем, принадлежит ли URL приложению
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        loginLogger.LogInformation("User logged in.");
                        return Redirect(model.ReturnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }

                return RedirectToAction("Login", new { status = AuthStatus.IncorrectData });
            }

            return RedirectToAction("Login", new { status = AuthStatus.NotRegistered });
        }

        return RedirectToAction("Login", new { status = AuthStatus.ValidationError });
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        // удаляем аутентификационные куки
        await signInManager.SignOutAsync();
        loginLogger.LogInformation("User logged out.");
        return RedirectToAction("Login", "Identity");
    }

    [HttpGet]
    public async Task<IActionResult> Register (RegisterStatus? status, string? returnUrl = null)
    {
        // FIXME (08.11.2023)
        // при открытии вьюхи сразу будут подсвечены ошибки валидации,
        // т.к. в параметрах метода передается модель. Подумать, как обойти

        // UPD (10.11.2023)
        // обошел, передаю теперь енам

        return View(new RegisterViewModel
        {
            ReturnUrl = returnUrl,
            ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList(),
            RegisterStatus = status ?? RegisterStatus.None
        });
    }

    [HttpPost]
    public async Task<IActionResult> Register (RegisterViewModel model)
    {
        // FIXME
        // если срабатывает валидация на стороне сервера, то в БД NLimit создается запись (а не должна!)

        var clietnIsPresent = await userManager.FindByNameAsync(model.Email);

        if (clietnIsPresent is not null)
        {
            return RedirectToAction("Register", new { status = RegisterStatus.AlreadyRegistered });
        }

        if (model.FirstName is null || model.Surname is null || model.BirthDate is null
            || model.Email is null || model.Password is null || model.ConfirmPassword is null)
        {
            return RedirectToAction("Register", new { status = RegisterStatus.IncorrectData });
        }

        /*if (!ModelState.IsValid)
        {
            model.RegisterStatus = RegisterStatus.ValidationError;
            return RedirectToAction("Register", model);
        }*/

        var user = new IdentityUser();
        try
        {
            user = Activator.CreateInstance<IdentityUser>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively");
        }

        // чтобы отрабатывала валидация на форме
        await userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);
        await emailStore.SetEmailAsync(user, model.Email, CancellationToken.None);

        // создаем юзера в БД identity
        var result = await userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            return RedirectToAction("Register", new { status = RegisterStatus.InternalServerError });
        }

        regLogger.LogInformation("User created a new account with password (Identity DB).");

        // создаем юзера в БД NLimit
        var responseCode = await CreateNLimitStudent(user, model);

        if (responseCode.ToString() != HttpStatusCode.Created.ToString())
        {
            await userManager.DeleteAsync(user);
            return RedirectToAction("Register", new { status = RegisterStatus.NLimitInternalServerError });
        }

        // генерация токена для пользователя
        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var callbackUrl = Url.Action(
            "ConfirmEmail",
            "Identity",
            values: new
            {
                userId = user.Id,
                code = code
            },
            protocol: Request.Scheme);

        EmailService emailService = new EmailService();
        await emailService.SendEmailAsync(model.Email, "Подтверждение email на NLimit",
            $"Добрый день! Вам направлено тестовое подтверждения. " +
            $"Пожалуйста, подтвердите регистрацию, перейдя по ссылке: <a href='{callbackUrl}'>link</a>");

        return RedirectToAction("Register", new { status = RegisterStatus.Success });
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail (string userId, string code)
    {
        if (userId is null || code is null)
        {
            return View("Error");
        }

        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return View("Error");
        }

        // проверяем соответствие токена пользователю
        var result = await userManager.ConfirmEmailAsync(user, code);
        if (!result.Succeeded)
        {
            return View("Error");
        }

        return RedirectToAction("Index", "Home");
    }

    private IUserEmailStore<IdentityUser> GetEmailStore()
    {
        if (!userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<IdentityUser>)userStore;
    }

    private async Task<string> CreateNLimitStudent(IdentityUser identityUser, RegisterViewModel model)
    {
        User user = new()
        {
            UserId = identityUser.Id,
            FirstName = model.FirstName,
            Surname = model.Surname,
            BirthDate = model.BirthDate, //DateTime.Now, // для теста, пока не выведу на UI поле 
            Email = identityUser.Email
        };

        string uri = "api/Users/CreateUser";

        HttpClient client = clientFactory.CreateClient(name: "NLimit.WebApi");
        HttpResponseMessage response = await client.PostAsJsonAsync(uri, user);
        var responseCode = response.StatusCode.ToString();

        return responseCode;
    }
}