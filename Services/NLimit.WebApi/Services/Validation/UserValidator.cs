using Data.NLimit.Common.EntitiesModels.SqlServer;
using FluentValidation;

namespace NLimit.WebApi.Services.Validation;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(u => u.UserId)
            .NotNull()
            .WithMessage("userId must not be null or empty")
            .NotEmpty()
            .WithMessage("userId must not be null or empty")
            .Length(50)
            .WithMessage("The length of the userId should not be more than 50");

        RuleFor(u => u.FirstName)
            .NotNull()
            .WithMessage("firstName must not be null or empty")
            .NotEmpty()
            .WithMessage("firstName must not be null or empty")
            .Length(30)
            .WithMessage("The length of the firstName should not be more than 30");

        RuleFor(u => u.Surname)
            .NotNull()
            .WithMessage("surname must not be null or empty")
            .NotEmpty()
            .WithMessage("surname must not be null or empty")
            .Length(30)
            .WithMessage("The length of the surname should not be more than 30");

        RuleFor(u => u.Patronymic)
            .Length(30)
            .WithMessage("The length of the patronymic should not be more than 30");
    }
}
