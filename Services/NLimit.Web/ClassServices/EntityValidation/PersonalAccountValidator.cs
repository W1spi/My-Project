using FluentValidation;
using NLimit.Web.Models;
using System.Web.WebPages;

namespace NLimit.Web.ClassServices.EntityValidation;

public class PersonalAccountValidator : AbstractValidator<PersonalAccountViewModel>
{
    public PersonalAccountValidator()
    {
        RuleFor(a => a.FirstName)
            .NotEmpty()
            .WithMessage("Поле [Имя] является обязательным")
            .NotNull()
            .WithMessage("Поле [Имя] является обязательным");
    }
}
