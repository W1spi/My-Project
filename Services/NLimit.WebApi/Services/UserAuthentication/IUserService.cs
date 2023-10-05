using NLimit.WebApi.Models;

namespace NLimit.WebApi.Services.UserAuthentication;

public interface IUserService
{
    bool IsValidUserInformation(LoginModel model, IConfiguration configuration);
    LoginModel GetUserDetails(string accountId, string accountPassword, IConfiguration congifuration);
}
