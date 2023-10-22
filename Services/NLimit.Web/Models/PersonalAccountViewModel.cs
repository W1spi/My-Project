using Data.NLimit.Common.EntitiesModels.SqlServer;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NLimit.Web.Models;

public class PersonalAccountViewModel : User
{
    // TODO: создать класс ApplicationUser и настроить ту модель вместо IdentityUser

    [JsonIgnore]
    public bool IsEmailConfirmed { get; set; }

    // флаг, указаывающий были ли обновлены данные 
    [JsonIgnore]
    public bool UpdatedSuccessfully { get; set; }

    [EmailAddress]
    [Display(Name = "Новый email")]
    public string? NewEmail { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [JsonIgnore]
    [Display(Name = "Текущий пароль")]
    public string CurrentPassword { get; set; }

    [JsonIgnore]
    public bool RequirePassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Старый пароль")]
    [JsonIgnore]
    public string OldPassword { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Новый пароль")]
    [JsonIgnore]
    public string NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Повторите новый пароль")]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    [JsonIgnore]
    public string ConfirmPassword { get; set; }

    [JsonIgnore]
    public ModelStates ModelIsValid { get; set; } = ModelStates.None;
}

public enum ModelStates
{
    None,

    InvalidNewEmail,
    ValidNewEmail,
    EmailsMatch,
    ConfirmationEmail,

    PasswordChanged,
    PasswordChangeError,

    ValidPassword,
    InvalidPassword
}