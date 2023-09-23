using System.ComponentModel.DataAnnotations;

namespace Data.NLimit.Common.EntitiesModels.SqlServer;

public class UserCourse
{
    public string UserId { get; set; }
    public string CourseId { get; set; }

    public double? AverageScore { get; set; }

    [DataType(DataType.Date)]
    public DateTime? EnrollmentDate { get; set; }

    public virtual User User { get; set; }
    public virtual Course Course { get; set; }
}
