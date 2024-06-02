using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NLimit.WebApi.Models;

namespace NLimit.WebApi.Services.Middleware;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var account = (LoginModel)context.HttpContext.Items["User"]!;
        if (account is null)
        {
            // not logged in
            context.Result = new JsonResult(new { code = 401, message = "Unauthorized" }) 
                { 
                    StatusCode = StatusCodes.Status401Unauthorized 
                };
        }
    }
}
