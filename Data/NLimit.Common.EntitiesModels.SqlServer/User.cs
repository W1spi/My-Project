using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FluentValidation.Attributes;
using LibraryOfUsefulClasses.EntityValidation;

namespace Data.NLimit.Common.EntitiesModels.SqlServer;

[Validator(typeof(UserValidator))]
public class User
{
    [Required]
    [StringLength(50)]
    public string UserId { get; set; }

    [Required(ErrorMessage = "Поле [Имя] является обязательным")]
    [StringLength(30, ErrorMessage = "Длина поля [Имя] должна быть не больше 30")]
    [Display(Name = "Имя")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Поле [Фамилия] является обязательным")]
    [StringLength(30, ErrorMessage = "Длина поля [Фамилия] должна быть не больше 30")]
    [Display(Name = "Фамилия")]
    public string Surname { get; set; }

    [StringLength(30, ErrorMessage = "Длина поля [Отчество] должна быть не больше 30")]
    [Display(Name = "Отчество")]
    public string? Patronymic { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Дата рождения")]
    // сделать обязательным, когда вернусь к работе над фронтом
    public DateTime? BirthDate { get; set; }

    [DataType(DataType.Date)]
    [JsonIgnore]
    public DateTime? StartDate { get; set; }

    [StringLength(100, ErrorMessage = "Длина поля [Адрес] должна быть не больше 100")]
    [Display(Name = "Адрес")]
    public string? Address { get; set; }

    [Required(ErrorMessage = "Поле [Email] является обязательным")]
    [StringLength(50, ErrorMessage = "Длина поля [Email] должна быть не больше 50")]
    [RegularExpression(@"^[-\w.]+@([A-Za-z0-9][-A-Za-z0-9]+\.)+[A-Za-z]{2,4}$", ErrorMessage = "Некорректное значение в поле [Email]")]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [StringLength(30, ErrorMessage = "Длина поля [Мобильный телефон] должна быть не больше 30")]
    [Display(Name = "Мобильный телефон")]
    public string? MobilePhone { get; set; }

    [StringLength(30, ErrorMessage = "Длина поля [Дополнительный телефон] должна быть не больше 30")]
    [Display(Name = "Дополнительный телефон")]
    public string? AdditionalPhone { get; set; }

    public virtual ICollection<Course>? Course { get; set; } = new List<Course>();

    [JsonIgnore]
    public virtual ICollection<UserCourse>? UserCourse { get; set; } = new List<UserCourse>()!;

    public virtual ICollection<Work>? Work { get; set; } = new List<Work>()!;
}

