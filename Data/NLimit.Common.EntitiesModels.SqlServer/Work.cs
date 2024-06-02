using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Data.NLimit.Common.EntitiesModels.SqlServer;

public class Work
{
    //[Key, StringLength(50), Column(TypeName = "nvarchar (50)")]
    public string WorkId { get; set; }

    //[Required, StringLength(100), Column(TypeName = "nvarchar (100)")]
    public string WorkName { get; set; }

    //[StringLength(255), Column(TypeName = "nvarchar (255)")]
    public string? WorkDescription { get; set; }

    //[StringLength(50), Column(TypeName = "nvarchar (50)")]
    public string? CreatedBy { get; set;}

    //[Required, StringLength(30), Column(TypeName = "nvarchar (30)")]
    public string WorkStatus { get; set; }

    //[StringLength(50), Column(TypeName = "nvarchar (50)")]
    public string? Executor { get; set; }

    //[StringLength(20), Column(TypeName = "nvarchar (20)")]
    public string? Result { get; set; }

    //[StringLength(255), Column(TypeName = "nvarchar (255)")]
    public string? FeedbackTeacher { get; set; }

    //[StringLength(50), Column(TypeName = "nvarchar (50)")]
    public string? UserId { get; set; }

    //[StringLength(50), Column(TypeName = "nvarchar (50)")]
    public string? CourseId { get; set; }

    [ForeignKey("UserId")]
    [JsonIgnore]
    public virtual User? User { get; set; }

    [ForeignKey("CourseId")]
    [JsonIgnore]
    public virtual Course? Course { get; set; }

}