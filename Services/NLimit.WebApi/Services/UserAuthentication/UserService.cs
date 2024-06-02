using Microsoft.Identity.Client;
using NLimit.WebApi.Models;

namespace NLimit.WebApi.Services.UserAuthentication;

public class UserService : IUserService
{
    public bool IsValidUserInformation(LoginModel model, IConfiguration configuration)
    {
        if (model.UserName == configuration["ApiAuth:FirstTestUser:UserName"] && model.Password == configuration["ApiAuth:FirstTestUser:Password"])
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public LoginModel GetUserDetails(string accountId, string accountPassword, IConfiguration configuration)
    {
        return new LoginModel { UserName = configuration["ApiAuth:FirstTestUser:UserName"], Password = configuration["ApiAuth:FirstTestUser:Password"] };
    }
}
