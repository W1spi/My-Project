using Data.NLimit.Common.EntitiesModels.SqlServer;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NLimit.Web.Models;

public class PersonalAccountViewModel : User
{
    // TODO: создать класс ApplicationUser и настроить ту модель вместо IdentityUser

    [Required(ErrorMessage = "Поле [{0}] является обязательным")]
    [DataType(DataType.Date)]
    [DisplayName("Дата рождения")]
    public DateTime? BirthDate { get; set; }

    [Required(ErrorMessage = "Поле [{0}] является обязательным")]
    [StringLength(50, ErrorMessage = "Длина поля [{0}] должна быть не больше 50")]
    [RegularExpression(@"^[-\w.]+@([A-Za-z0-9][-A-Za-z0-9]+\.)+[A-Za-z]{2,4}$", ErrorMessage = "Некорректное значение в поле [{0}]")]
    [DisplayName("Новый email")]
    public string NewEmail { get; set; }

    [Required(ErrorMessage = "Поле [{0}] является обязательным")]
    [DataType(DataType.Password)]
    [JsonIgnore]
    [Display(Name = "Текущий пароль")]
    public string CurrentPassword { get; set; }

    [Required(ErrorMessage = "Поле [{0}] является обязательным")]
    [StringLength(100, ErrorMessage = "Поле [{0}] должно содержать не менее {2} и не более {1} символов", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Новый пароль")]
    [Compare("ConfirmPassword", ErrorMessage = "Введенное значение не совпадает со значением поля [Повторите новый пароль]")]
    [JsonIgnore]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "Поле [{0}] является обязательным")]
    [StringLength(100, ErrorMessage = "Поле [{0}] должно содержать не менее {2} и не более {1} символов", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Повторите новый пароль")]
    [Compare("NewPassword", ErrorMessage = "Введенное значение не совпадает со значением поля [Новый пароль]")]
    [JsonIgnore]
    public string ConfirmPassword { get; set; }

    [JsonIgnore]
    public bool RequirePassword { get; set; }

    [JsonIgnore]
    public bool IsEmailConfirmed { get; set; }

    // флаг, указаывающий были ли обновлены данные 
    [JsonIgnore]
    public bool UpdatedSuccessfully { get; set; }

    [JsonIgnore]
    public ModelStates ModelIsValid { get; set; }
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