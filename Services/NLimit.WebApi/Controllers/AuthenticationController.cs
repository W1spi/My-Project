using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using NLimit.WebApi.Models;
using NLimit.WebApi.Services.UserAuthentication;
using System.Text;
using Data.NLimit.Common.EntitiesModels.SqlServer;
using NLimit.WebApi.Services.ResponseTemplates;

namespace NLimit.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IUserService userService;

        public AuthenticationController(IConfiguration configuration, IUserService userService)
        {
            this.configuration = configuration;
            this.userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(CustomResponseExamplesBadRequest))]
        public IActionResult Authentication([FromBody] LoginModel model)
        {
            bool isValid = userService.IsValidUserInformation(model, configuration);

            if (!isValid)
            {
                //return BadRequest();
                return new JsonResult(new {code = 400, message = "Invalid userName or password"}) { StatusCode = StatusCodes.Status400BadRequest };
            }

            var tokenString = GenerateJwtToken(model.UserName);
            return Ok(new
            {
                Token = tokenString,
                Message = "Success"
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet(nameof(GetResult))]
        public IActionResult GetResult()
        {
            return new JsonResult(new { code = 200, message = "API validated" }) { StatusCode = StatusCodes.Status200OK };
        }

        /// <summary>
        /// Generate JWT Token after successful login.
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        private string GenerateJwtToken(string userName)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration["Jwt:key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", userName)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = configuration["Jwt:Issuer"],
                Audience = configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
