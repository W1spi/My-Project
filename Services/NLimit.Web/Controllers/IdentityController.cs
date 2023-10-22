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
    public async Task<IActionResult> Login(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        return View(new LoginViewModel
        {
            ReturnUrl = returnUrl
        });
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        model.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        if (ModelState.IsValid)
        {
            // ищем пользователя
            var user = await userManager.FindByNameAsync(model.Email);
            if (user is not null)
            {
                // проверяем, подтвержден ли email
                if (!await userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError(string.Empty, "Email не был подтвержден");
                    return View(model);
                }
            }

            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // проверяем, принадлежит ли URL приложению
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    loginLogger.LogInformation("User logged in.");
                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Введены некорректные данные! Повторите попытку.");
            }
        }
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        // удаляем аутентификационные куки
        await signInManager.SignOutAsync();
        loginLogger.LogInformation("User logged out.");
        return RedirectToAction("Login", "Identity");
    }

    // предыдущая версия метода Logout 
    /*[HttpPost]
    public async Task<IActionResult> Logout(string? returnUrl = null)
    {
        // удаляем аутентификационные куки
        await signInManager.SignOutAsync();
        loginLogger.LogInformation("User logged out.");

        if (returnUrl is not null)
        {
            return LocalRedirect(returnUrl);
        }
        else
        {
            return RedirectToAction("Login", "Identity");
        }
    }*/

    [HttpGet]
    public async Task<IActionResult> Register (string? returnUrl = null)
    {
        return View(new RegisterViewModel
        {
            ReturnUrl = returnUrl,
            ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
        });
    }

    [HttpPost]
    public async Task<IActionResult> Register (RegisterViewModel? model)
    {
        // FIXME
        // если срабатывает валидация на стороне сервера, то в БД NLimit создается запись (а не должна!)

        var clietnIsPresent = await userManager.FindByNameAsync(model.Email);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (clietnIsPresent is not null)
        {
            return BadRequest("По указанному email уже зарегистрирован пользователь!");
        }

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
            return BadRequest("Клиент не был создан (что-то пошло не так с identity)");
        }

        regLogger.LogInformation("User created a new account with password (Identity DB).");

        // создаем юзера в БД NLimit
        var responseCode = await CreateNLimitStudent(user, model);

        if (responseCode.ToString() != HttpStatusCode.Created.ToString())
        {
            return BadRequest("Клиент не был создан (что-то пошло не так с NLimit)");
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

        return Content("Для завершения регистрации проверьте электронную почту и перейдите по ссылке, указанной в письме.");
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
        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Home");
        }
        else
        {
            return View("Error");
        }
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
            BirthDate = DateTime.Now, // для теста, пока не выведу на UI поле 
            Email = identityUser.Email
        };

        string uri = "api/Users/CreateUser";

        HttpClient client = clientFactory.CreateClient(name: "NLimit.WebApi");
        HttpResponseMessage response = await client.PostAsJsonAsync(uri, user);
        var responseCode = response.StatusCode.ToString();

        return responseCode;
    }
}