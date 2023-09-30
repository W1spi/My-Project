using Data.NLimit.Common.EntitiesModels.SqlServer;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Data.NLimit.Common.EntitiesModels.SqlServer;

public class User
{
    //[Key, StringLength(50), Column(TypeName = "nvarchar (50)")]
    [Required(ErrorMessage = "Поле [UserId] является обязательным.")]
    public string UserId { get; set; }

    //[Required, StringLength(30), Column(TypeName = "nvarchar (30)")]
    public string FirstName { get; set; }

    //[Required, StringLength(30), Column(TypeName = "nvarchar (30)")]
    public string Surname { get; set; }

    //[StringLength(30), Column(TypeName = "nvarchar (30)")]
    public string? Patronymic { get; set; }

    [DataType(DataType.Date)]
    public DateTime? BirthDate { get; set; }

    [DataType(DataType.Date)]
    [JsonIgnore]
    public DateTime? StartDate { get; set; }

    //[StringLength(100), Column(TypeName = "nvarchar (100)")]
    public string? Address { get; set; }

    //[Required, StringLength(50), DataType(DataType.EmailAddress), Column(TypeName = "nvarchar (50)")]
    public string Email { get; set; }

    //[StringLength(30), DataType(DataType.PhoneNumber), Column(TypeName = "nvarchar (30)")]
    public string? MobilePhone { get; set; }

    //[StringLength(30), DataType(DataType.PhoneNumber), Column(TypeName = "nvarchar (30)")]
    public string? AdditionalPhone { get; set; }

    public virtual ICollection<Course>? Course { get; set; } = new List<Course>();

    [JsonIgnore]
    public virtual ICollection<UserCourse>? UserCourse { get; set; } = new List<UserCourse>()!;
    //[JsonIgnore]
    public virtual ICollection<Work>? Work { get; set; } = new List<Work>()!;
}

