using Data.NLimit.Common.EntitiesModels.SqlServer;
using FluentValidation;

namespace NLimit.WebApi.Services.Validation;

public class WorkValidator : AbstractValidator<Work>
{
    public WorkValidator()
    {
        RuleFor(w => w.WorkId)
            .NotNull()
            .WithMessage("workId must not be null or empty")
            .NotEmpty()
            .WithMessage("workId must not be null or empty")
            .MaximumLength(50)
            .WithMessage("The length of the workId should not be more than 50");

        RuleFor(w => w.WorkName)
            .NotNull()
            .WithMessage("workName must not be null or empty")
            .NotEmpty()
            .WithMessage("workName must not be null or empty")
            .MaximumLength(100)
            .WithMessage("The length of the workName should not be more than 100");

        RuleFor(w => w.WorkDescription)
            .MaximumLength(255)
            .WithMessage("The length of the workDescription should not be more than 255");

        RuleFor(w => w.CreatedBy)
            .MaximumLength(50)
            .WithMessage("The length of the createdBy should not be more than 50");

        RuleFor(w => w.WorkStatus)
            .NotNull()
            .WithMessage("workStatus must not be null or empty")
            .NotEmpty()
            .WithMessage("workStatus must not be null or empty")
            .MaximumLength(30)
            .WithMessage("The length of the workStatus should not be more than 30");

        RuleFor(w => w.Executor)
            .MaximumLength(50)
            .WithMessage("The length of the executor should not be more than 50");

        RuleFor(w => w.Result)
            .MaximumLength(20)
            .WithMessage("The length of the result should not be more than 20");

        RuleFor(w => w.FeedbackTeacher)
            .MaximumLength(255)
            .WithMessage("The length of the feedbackTeacher should not be more than 255");

        RuleFor(w => w.UserId)
            .MaximumLength(50)
            .WithMessage("The length of the userId should not be more than 50");

        RuleFor(w => w.CourseId)
            .MaximumLength(50)
            .WithMessage("The length of the courseId should not be more than 50");
    }
}
