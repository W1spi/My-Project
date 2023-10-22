using Data.NLimit.Common.DataContext.SqlServer;
using Data.NLimit.Common.EntitiesModels.SqlServer;
using FluentValidation; // кастомная валидация
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NLimit.Web.AppServices;
using NLimit.Web.Data;
using NLimit.Web.Models;
using Org.BouncyCastle.Asn1.X509.SigI;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Web.WebPages;

namespace NLimit.Web.Areas.MvcPages.Controllers
{
    public class PersonalDataController : Controller
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly NLimitContext db;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly ILogger<PersonalDataController> logger;
        private readonly IValidator<PersonalAccountViewModel> validator;

        public PersonalDataController(NLimitContext injectedContext, SignInManager<IdentityUser> signInManager, 
            UserManager<IdentityUser> userManager, IHttpClientFactory httpClientFactory, 
            ILogger<PersonalDataController> logger, IValidator<PersonalAccountViewModel> validator)
        {
            clientFactory = httpClientFactory;
            db = injectedContext;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.logger = logger;
            this.validator = validator;
        }


        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var identityUser = await userManager.GetUserAsync(User);

            if (identityUser is null)
            {
                return NotFound($"Unable to load user with ID '{identityUser.Id}'.");
            }

            // TODO: пока отталкиваюсь от того, что у пользака точно будет присутствовать запись в обеих БД
            User? user = await GetUser(identityUser.Id);

            if (user is null)
            {
                return NotFound($"Юзер не найден");
            }

            var model = new PersonalAccountViewModel
            {
                FirstName = user.FirstName,
                Surname = user.Surname,
                Patronymic = user.Patronymic!,
                BirthDate = user.BirthDate ?? DateTime.MinValue,
                MobilePhone = user.MobilePhone!,
                Address = user.Address!
            };

            return View("~/Areas/MvcPages/Views/PersonalData/Profile.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(PersonalAccountViewModel? model)
        {
            // TODO: пока убираю проверка модели, т.к. там нужно править. Сейчас не заходим в if,
            //       т.к. не проходит валидацию email (в модели висит атрибут обязательности, 
            //       а с формы уходит null

            if (model is null)
            {
                return BadRequest();
            }

            var identityUser = await userManager.GetUserAsync(User);
            model.UserId = identityUser.Id;

            var validationResult = await validator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                // берет первую ошибку валидации
                var query = (from errors in validationResult.Errors
                             select errors.ErrorMessage)
                            .First();

                return View("~/Areas/MvcPages/Views/PersonalData/Profile.cshtml", model);
            }

            HttpStatusCode responseCode = await UpdateProfileUser(identityUser.Id, model.FirstName, model.Surname, model.Patronymic,
             model.BirthDate, model.MobilePhone, model.Address);

            if (responseCode != HttpStatusCode.OK)
            {
                return BadRequest(responseCode.ToString() + " " + "Произошла ошибка при обновлении");
            }

            model.UpdatedSuccessfully = true;
            return View("~/Areas/MvcPages/Views/PersonalData/Profile.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> Email()
        {
            //string userId = userManager.GetUserId(User)!;
            var identityUser = await userManager.GetUserAsync(User);

            if (identityUser is not null)
            {
                // TODO: пока отталкиваюсь от того, что у пользака точно будет присутствовать запись в обеих БД
                User? user = await GetUser(identityUser.Id);

                if (user is null)
                {
                    return NotFound($"Unable to load user with ID '{identityUser.Id}'.");
                }

                var model = new PersonalAccountViewModel
                {
                    Email = user.Email!,
                    IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(identityUser)
                };


                return View("~/Areas/MvcPages/Views/PersonalData/Email.cshtml", model);
            }
            else
            {
                return NotFound($"Unable to load user with ID '{identityUser.Id}'.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeEmail(string newEmail)
        {
            PersonalAccountViewModel model = new();
            var identityUser = await userManager.GetUserAsync(User);
            var currentEmail = await userManager.GetEmailAsync(identityUser);

            if (identityUser is null)
            {
                return NotFound($"Пользователь с id '{identityUser.Id}' не найден");
            }

            if (newEmail == currentEmail)
            {
                model.ModelIsValid = ModelStates.EmailsMatch;
                model.Email = identityUser.Email;
                model.IsEmailConfirmed = identityUser.EmailConfirmed;
                return View("~/Areas/MvcPages/Views/PersonalData/Email.cshtml", model);
            }

            if (newEmail is null || !ModelState.IsValid)
            {
                model.ModelIsValid = ModelStates.InvalidNewEmail;
                model.Email = identityUser.Email;
                model.IsEmailConfirmed = identityUser.EmailConfirmed;
                return View("~/Areas/MvcPages/Views/PersonalData/Email.cshtml", model);
            }

            // обновляем email в БД NLimit
            var responseCode = await UpdateEmail(identityUser.Id, newEmail);

            if (responseCode != HttpStatusCode.NoContent.ToString())
            {
                model.ModelIsValid = ModelStates.InvalidNewEmail;
                return View("~/Areas/MvcPages/Views/PersonalData/Email.cshtml", model);
            }

            // если все ок, то обновляем email в БД Identity, стафим флаг IsConfirmed = false и кидаем алерт об успехе
            model.ModelIsValid = ModelStates.ValidNewEmail;
            model.IsEmailConfirmed = false;
            model.Email = newEmail;
            identityUser.EmailConfirmed = false;

            await UpdateIdentityEmail(identityUser, newEmail);

            return View("~/Areas/MvcPages/Views/PersonalData/Email.cshtml", model);
        }

        //[HttpGet]
        public async Task<IActionResult> ConfirmEmailChange()
        {
            var identityUser = await userManager.GetUserAsync(User);

            var code = await userManager.GenerateEmailConfirmationTokenAsync(identityUser);
            var callbackUrl = Url.Action(
                "ConfirmEmail",
                "Identity",
                values: new
                {
                    userId = identityUser.Id,
                    code = code
                },
                protocol: Request.Scheme);

            EmailService emailService = new EmailService();
            await emailService.SendEmailAsync(identityUser.Email, "Подтверждение email на NLimit",
                $"Добрый день! Вам направлено тестовое подтверждения. " +
                $"Пожалуйста, подтвердите ваш новый email, перейдя по ссылке: <a href='{callbackUrl}'>link</a>");

            PersonalAccountViewModel model = new()
            {
                ModelIsValid = ModelStates.ConfirmationEmail,
                Email = identityUser.Email
            };

            model.ModelIsValid = ModelStates.ConfirmationEmail;
            return View("~/Areas/MvcPages/Views/PersonalData/Email.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            string userId = userManager.GetUserId(User)!;

            if (userId is null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            User? user = await GetUser(userId);

            if (user is null || userId != user.UserId)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            PersonalAccountViewModel model = new()
            {
                ModelIsValid = ModelStates.None
            };

            return View("~/Areas/MvcPages/Views/PersonalData/ChangePassword.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            PersonalAccountViewModel model = new();

            if (!ModelState.IsValid)
            {
                model.ModelIsValid = ModelStates.PasswordChangeError;
                return View("~/Areas/MvcPages/Views/PersonalData/ChangePassword.cshtml", model);
            }

            var identityUser = await userManager.GetUserAsync(User);
            if (identityUser is null)
            {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await userManager.ChangePasswordAsync(identityUser, oldPassword, newPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                model.ModelIsValid = ModelStates.PasswordChangeError;
                return View("~/Areas/MvcPages/Views/PersonalData/ChangePassword.cshtml", model);
            }

            await signInManager.RefreshSignInAsync(identityUser);
            logger.LogInformation("User changed their password successfully.");

            model.ModelIsValid = ModelStates.PasswordChanged;
            return View("~/Areas/MvcPages/Views/PersonalData/ChangePassword.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> PersData()
        {
            //string userId = userManager.GetUserId(User)!;
            var identityUser = await userManager.GetUserAsync(User);

            if (identityUser is null)
            {
                return NotFound($"Unable to load user with ID '{identityUser.Id}'.");
            }

            User? user = await GetUser(identityUser.Id);

            if (user.UserId != identityUser.Id)
            {
                return NotFound($"Unable to load user with ID '{identityUser.Id}'.");
            }

            return View("~/Areas/MvcPages/Views/PersonalData/PersData.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> DownloadPersData()
        {
            var identityUser = await userManager.GetUserAsync(User);

            if (identityUser is null)
            {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            logger.LogInformation("User with ID '{UserId}' asked for their personal data.", userManager.GetUserId(User));

            // Only include personal data for download
            var personalData = new Dictionary<string, string>();

            var personalDataProps = typeof(IdentityUser)
                    .GetProperties()
                    .Where(prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));

            foreach (var p in personalDataProps)
            {
                personalData.Add(p.Name, p.GetValue(identityUser)?.ToString() ?? "null");
            }

            var logins = await userManager.GetLoginsAsync(identityUser);

            foreach (var l in logins)
            {
                personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
            }

            personalData.Add($"Authenticator Key", await userManager.GetAuthenticatorKeyAsync(identityUser));

            Response.Headers.Add("Content-Disposition", "attachment; filename=PersonalData.json");
            return new FileContentResult(JsonSerializer.SerializeToUtf8Bytes(personalData), "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> DeletePersData()
        {
            var identityUser = await userManager.GetUserAsync(User);

            if (identityUser is null)
            {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            PersonalAccountViewModel model = new()
            {
                RequirePassword = await userManager.HasPasswordAsync(identityUser)
            };

            return View("~/Areas/MvcPages/Views/PersonalData/DeletePersData.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePersData(string currentPassword)
        {
            var identityUser = await userManager.GetUserAsync(User);

            if (identityUser is null)
            {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            PersonalAccountViewModel model = new();

            if (!await userManager.CheckPasswordAsync(identityUser, currentPassword))
            {
                //ModelState.AddModelError(string.Empty, "Incorrect password.");
                model.ModelIsValid = ModelStates.InvalidPassword;
                model.RequirePassword = await userManager.HasPasswordAsync(identityUser);
                return View("~/Areas/MvcPages/Views/PersonalData/DeletePersData.cshtml", model);
            }

            var result = await userManager.DeleteAsync(identityUser);
            var userId = await userManager.GetUserIdAsync(identityUser);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred deleting user.");
            }

            await signInManager.SignOutAsync();

            logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);

            return Redirect("~/");
        }


        private async Task<User> GetUser(string userId)
        {
            string uri = $"api/Users/GetOneUser/{userId}";

            HttpClient client = clientFactory.CreateClient("NLimit.WebApi");

            HttpRequestMessage request = new(
                method: HttpMethod.Get,
                requestUri: uri);

            HttpResponseMessage response = await client.SendAsync(request);

            User? user = await response.Content.ReadFromJsonAsync<User>();

            return user!;
        }

        private async Task UpdateIdentityEmail(IdentityUser identityUser, string newEmail)
        {
            await userManager.SetEmailAsync(identityUser, newEmail);
            await userManager.SetUserNameAsync(identityUser, newEmail);
            await userManager.UpdateNormalizedEmailAsync(identityUser);
            await userManager.UpdateNormalizedUserNameAsync(identityUser);
        }

        private async Task<string> UpdateUser(User? user)
        {
            string uri = $"api/Users/UpdateUser";

            HttpClient client = clientFactory.CreateClient("NLimit.WebApi");

            HttpResponseMessage response = await client.PutAsJsonAsync(uri, user);

            var responseCode = response.StatusCode.ToString();

            return responseCode;
        }

        private async Task<HttpStatusCode> UpdateProfileUser(string id, string firstName, string surname, string? patronymic,
        DateTime? birthDate, string? mobilePhone, string? address)
        {
            string uri = $"api/Users/UpdateProfileUser/{id}?firstName={firstName}&surname={surname}&patronymic={patronymic}" +
                $"&birthDate={birthDate}&mobilePhone={mobilePhone}&address={address}";

            HttpClient client = clientFactory.CreateClient("NLimit.WebApi");

            HttpRequestMessage request = new(
                method: HttpMethod.Put,
                requestUri: uri);

            HttpResponseMessage response = await client.SendAsync(request);

            var responseCode = response.StatusCode;

            return responseCode;
        }

        private async Task<string> UpdateEmail(string id, string newEmail)
        {
            string uri = $"api/Users/UpdateEmail/{id}?newEmail={newEmail}";

            HttpClient client = clientFactory.CreateClient("NLimit.WebApi");

            HttpRequestMessage request = new(
                method: HttpMethod.Put,
                requestUri: uri);

            HttpResponseMessage response = await client.SendAsync(request);

            var responseCode = response.StatusCode.ToString();

            return responseCode;
        }
    }
}
