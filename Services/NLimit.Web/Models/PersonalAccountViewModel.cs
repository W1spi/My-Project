using System.ComponentModel.DataAnnotations;

namespace NLimit.Web.Models;

public class PersonalAccountViewModel
{
    // TODO: создать класс ApplicationUser и настроить ту модель вместо IdentityUser

    public string Id { get; set; }

    [Required(ErrorMessage = $"Поле является обязательным")]
    public string FirstName { get; set; }

    [Required]
    public string Surname { get; set; }

    public string? Patronymic { get; set; }

    public DateTime? BirthDate { get; set; }

    //[Required]
    [MaxLength(24)]
    public string Email { get; set; }

    [MaxLength(60)]
    public string? Address { get; set; }

    [MaxLength(24)]
    public string? MobilePhone { get; set; }

    public bool IsEmailConfirmed { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; }

    public bool RequirePassword { get; set; }

    // флаг, указаывающий были ли обновлены данные 
    public bool UpdatedSuccessfully { get; set; }

    [EmailAddress]
    [Display(Name = "New email")]
    public string? NewEmail { get; set; }

    public ModelStates ModelIsValid { get; set; } = ModelStates.None;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Current password")]
    public string OldPassword { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "New password")]
    public string NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm new password")]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
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