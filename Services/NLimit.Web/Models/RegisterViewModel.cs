using Data.NLimit.Common.EntitiesModels.SqlServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace NLimit.Web.Models;

public class RegisterViewModel : User
{
    [Required(ErrorMessage = "Поле [Пароль] является обязательным")]
    [StringLength(100, ErrorMessage = "Длина поля [Пароль] не должна превышать 100 символов")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    [Compare("ConfirmPassword", ErrorMessage = "Введенные пароли не совпадают")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Поле [Повторите пароль] является обязательным")]
    [DataType(DataType.Password)]
    [Display(Name = "Повторите пароль")]
    [Compare("Password", ErrorMessage = "Введенные пароли не совпадают")]
    public string ConfirmPassword { get; set; }

    public RegisterStatus RegisterStatus { get; set; }

    public string? ReturnUrl { get; set; }
    public IList<AuthenticationScheme>? ExternalLogins { get; set; }

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
}

public enum RegisterStatus : sbyte
{
    Success = 1,

    AlreadyRegistered,
    IncorrectData,
    ValidationError,

    InternalServerError,
    NLimitInternalServerError,

    None
}