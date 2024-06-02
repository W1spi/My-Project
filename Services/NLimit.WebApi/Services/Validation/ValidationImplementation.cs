using Data.NLimit.Common.EntitiesModels.SqlServer;
using FluentValidation;

namespace NLimit.WebApi.Services.Validation;

public class ValidationImplementation<T>
{
    public static async Task<bool> IsValidModel (IValidator<T> validator, T obj)
    {
        var validationResult = await validator.ValidateAsync(obj);

        var query = (from errors in validationResult.Errors
                     select errors.ErrorMessage)
                            .First();

        return true;// не дореализовал, в идеале обобщить и на user, и на work.
    }
}
