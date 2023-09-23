using System.ComponentModel.DataAnnotations;

namespace NLimit.Web.Models;

public class AboutWorkViewModel
{
    public string WorkId { get; set; }

    public string WorkName { get; set; }

    public string? WorkDescription { get; set; }

    public string? CreatedBy { get; set; }

    public string WorkStatus { get; set; }

    public string? Executor { get; set; }

    public string? Result { get; set; }

    public string? FeedbackTeacher { get; set; }
}