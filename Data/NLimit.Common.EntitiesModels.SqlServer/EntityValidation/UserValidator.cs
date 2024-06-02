using FluentValidation;
using Data.NLimit.Common.EntitiesModels.SqlServer;

namespace LibraryOfUsefulClasses.EntityValidation;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(a => a.FirstName)
            .NotEmpty()
            .WithMessage("Поле [Имя] является обязательным (FL)")
            .NotNull()
            .WithMessage("Поле [Имя] является обязательным (FL)");
    }
}
