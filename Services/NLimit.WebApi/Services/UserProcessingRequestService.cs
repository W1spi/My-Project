using Data.NLimit.Common.EntitiesModels.SqlServer;
using Microsoft.OpenApi.Models;

namespace NLimit.WebApi.Services;

public static class UserProcessingRequestService
{
    public static User ProcessingRequestParameters(User user, UserOperationType type)
    {
        if (type  == UserOperationType.Create)
        {
            if (string.IsNullOrEmpty(user.Patronymic))
            {
                user.Patronymic = null;
            }
            if (string.IsNullOrEmpty(user.Address))
            {
                user.Address = null;
            }
            if (string.IsNullOrEmpty(user.Address))
            {
                user.Address = null;
            }
            if (string.IsNullOrEmpty(user.MobilePhone))
            {
                user.MobilePhone = null;
            }
            if (string.IsNullOrEmpty(user.AdditionalPhone))
            {
                user.AdditionalPhone = null;
            }

            return user;
        }
        else if (type == UserOperationType.Update)
        {
            return user;
        }
        else
        {
            return null;
        }
    }

    public enum UserOperationType
    {
        Create = 1,
        Update = 2
    }
}
