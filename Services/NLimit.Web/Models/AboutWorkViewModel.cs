using Data.NLimit.Common.EntitiesModels.SqlServer;
using System.ComponentModel.DataAnnotations;

namespace NLimit.Web.Models;

public class AboutWorkViewModel : Work
{
    public string WorkId { get; set; }

    public string WorkName { get; set; }

    public string? WorkDescription { get; set; }

    public string? CreatedBy { get; set; }

    public string WorkStatus { get; set; }

    public string? Executor { get; set; }

    public string? Result { get; set; }

    public string? FeedbackTeacher { get; set; }

    public string? UserId { get; set; }

    public string? CourseId { get; set; }

    public bool? WorkIsPresent { get; set; } // флаг присутствия заданий

    public IEnumerable<AboutWorkViewModel>? AllWorks { get; set; } 
}