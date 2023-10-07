using Data.NLimit.Common.EntitiesModels.SqlServer;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NLimit.Web.Models;

public class PersonalAccountViewModel : User
{
    // TODO: создать класс ApplicationUser и настроить ту модель вместо IdentityUser

    [JsonIgnore]
    public bool IsEmailConfirmed { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [JsonIgnore]
    public string CurrentPassword { get; set; }

    [JsonIgnore]
    public bool RequirePassword { get; set; }

    // флаг, указаывающий были ли обновлены данные 
    [JsonIgnore]
    public bool UpdatedSuccessfully { get; set; }

    [EmailAddress]
    [Display(Name = "New email")]
    public string? NewEmail { get; set; }

    [JsonIgnore]
    public ModelStates ModelIsValid { get; set; } = ModelStates.None;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Current password")]
    [JsonIgnore]
    public string OldPassword { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "New password")]
    [JsonIgnore]
    public string NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm new password")]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    [JsonIgnore]
    public string ConfirmPassword { get; set; }
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