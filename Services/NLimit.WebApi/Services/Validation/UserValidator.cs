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
            .MaximumLength(50)
            .WithMessage("The length of the userId should not be more than 50");

        RuleFor(u => u.FirstName)
            .NotNull()
            .WithMessage("firstName must not be null or empty")
            .NotEmpty()
            .WithMessage("firstName must not be null or empty")
            .MaximumLength(30)
            .WithMessage("The length of the firstName should not be more than 30");

        RuleFor(u => u.Surname)
            .NotNull()
            .WithMessage("surname must not be null or empty")
            .NotEmpty()
            .WithMessage("surname must not be null or empty")
            .MaximumLength(30)
            .WithMessage("The length of the surname should not be more than 30");

        RuleFor(u => u.Patronymic)
            .MaximumLength(30)
            .WithMessage("The length of the patronymic should not be more than 30");

        RuleFor(u => u.BirthDate)
            .NotEmpty()
            .WithMessage("birthDate must not be empty");

        RuleFor(u => u.Address)
            .MaximumLength(100)
            .WithMessage("The length of the address should not be more than 100");

        RuleFor(u => u.Email)
            .MaximumLength(50)
            .WithMessage("The length of the email should not be more than 50")
            .Matches(@"^[-\w.]+@([A-Za-z0-9][-A-Za-z0-9]+\.)+[A-Za-z]{2,4}$")
            .WithMessage("Invalid email address value entered");

        RuleFor(u => u.MobilePhone)
            .MaximumLength(30)
            .WithMessage("The length of the mobilePhone should not be more than 30");

        RuleFor(u => u.AdditionalPhone)
            .MaximumLength(30)
            .WithMessage("The length of the additionalPhone should not be more than 30");
    }
}
