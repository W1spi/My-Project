using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;
using System.Web.Mvc;
using Data.NLimit.Common.EntitiesModels.SqlServer;
using System.ComponentModel;
//using System.Web.Mvc;

namespace NLimit.Web.Models;

public class LoginViewModel : User
{
    [Required(ErrorMessage = "Поле [Пароль] является обязательным")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Display(Name = "Запомнить меня?")]
    public bool RememberMe { get; set; }

    public IList<AuthenticationScheme>? ExternalLogins { get; set; }

    public string? ReturnUrl { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    private IdentityUser CreateUser()
    {
        try
        {
            return Activator.CreateInstance<IdentityUser>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
        }
    }

    public AuthStatus AuthStatus { get; set; }
}

public enum AuthStatus : byte
{
    ValidationError,
    EmailNotConfirmed,
    IncorrectData,
    NotRegistered,

    None
}

