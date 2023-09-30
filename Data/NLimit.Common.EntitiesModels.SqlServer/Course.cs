using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Data.NLimit.Common.EntitiesModels.SqlServer;

public class Course
{
    //[Key, StringLength(50), Column(TypeName = "nvarchar (50)")]
    public string? CourseId { get; set; }

    //[Required, StringLength(100), Column(TypeName = "nvarchar (100)")]
    public string? CourseName { get; set; }

    //[StringLength(255), Column(TypeName = "nvarchar (255)")]
    public string? CourseDescription { get; set; }

    [JsonIgnore]
    public virtual ICollection<User>? User { get; set; } = new List<User>();

    [JsonIgnore]
    public virtual ICollection<UserCourse>? UserCourse { get; set; } = new List<UserCourse>();
    [JsonIgnore]
    public virtual ICollection<Work>? Work { get; set; } = new List<Work>();
}
