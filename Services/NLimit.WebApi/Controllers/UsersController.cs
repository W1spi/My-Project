using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Data.NLimit.Common.EntitiesModels.SqlServer;
using System.Net;
using System.ComponentModel.DataAnnotations;
using NLimit.WebApi.Repositoires.Users;
using LibraryOfUsefulClasses.Extensions;
using Microsoft.AspNetCore.Authorization;
using NLimit.WebApi.Services.Middleware;
using NLimit.WebApi.Services.ResponseTemplates;
using Swashbuckle.AspNetCore.Filters;
using System.Web.WebPages;
using FluentValidation;
using System.ComponentModel;

namespace NLimit.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository repo;
        private readonly IValidator<User> validator;

        public UsersController(IUserRepository repo, IValidator<User> validator)
        {
            this.repo = repo;
            this.validator = validator;
        }

        [HttpGet("GetAllUsers")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<User>))]
        public async Task<IEnumerable<User>> GetUsers(string? firstName)
        {
            if (string.IsNullOrEmpty(firstName))
            {
                return await repo.RetrieveAllAsync();
            }

            return (await repo.RetrieveAllAsync())
                    .Where(s => s.FirstName == firstName);
        }

        [HttpGet("GetOneUser/{id}", Name = nameof(GetUsers))]
        [ProducesResponseType(200, Type = typeof(User))]
        [ProducesResponseType(404, Type = typeof(CustomResponseExamplesNotFound))]
        public async Task<IActionResult> GetUser(string id)
        {
            User? user = await repo.RetrieveAsync(id);

            if (user is null)
            {
                return NotFound(new CustomResponseExamplesNotFound(StatusCodes.Status404NotFound, "User not found"));
            }
            return Ok(user);
        }

        [HttpPost("CreateUser", Name = nameof(GetUser))]
        [ProducesResponseType(201, Type = typeof(User))]
        [ProducesResponseType(400, Type = typeof(CustomResponseExamplesBadRequest))]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            if (user is null)
            {
                return BadRequest(new CustomResponseExamplesBadRequest(StatusCodes.Status400BadRequest, "The request body should not be empty"));
            }

            var validationResult = await validator.ValidateAsync(user);
            if (!validationResult.IsValid)
            {
                // берет первую ошибку валидации
                var query = (from errors in validationResult.Errors
                            select errors.ErrorMessage)
                            .First();

                return BadRequest(new CustomResponseExamplesBadRequest(StatusCodes.Status400BadRequest, query));
            }

            // постобработчик при создании записей
            user.EmptyToNull();

            User? addedUser = await repo.CreateAsync(user);
            if (addedUser is null)
            {
                return BadRequest(new CustomResponseExamplesBadRequest(StatusCodes.Status400BadRequest, "Repository failed to create User"));
            }

            return CreatedAtRoute(
                routeName: nameof(GetUsers),
                routeValues: new
                {
                    id = addedUser.UserId.ToLower()
                },
                value: addedUser);
        }

        // PUT: api/Users/UpdateUser - полное обновление пользователя
        [HttpPut("UpdateUser")]
        [ProducesResponseType(204, Type = typeof(User))]
        [ProducesResponseType(400, Type = typeof(CustomResponseExamplesBadRequest))]
        [ProducesResponseType(404, Type = typeof(CustomResponseExamplesNotFound))]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            if (user is null || user.UserId is null)
            {
                return BadRequest(new CustomResponseExamplesBadRequest(StatusCodes.Status400BadRequest, "The request body must not be empty, and the id must also match"));
            }

            var validationResult = await validator.ValidateAsync(user);
            if (!validationResult.IsValid)
            {
                var query = (from errors in validationResult.Errors
                             select errors.ErrorMessage)
                            .First();

                return BadRequest(new CustomResponseExamplesBadRequest(StatusCodes.Status400BadRequest, query));
            }

            User? existing = await repo.RetrieveAsync(user.UserId);

            if (existing is null)
            {
                return NotFound(new CustomResponseExamplesNotFound(StatusCodes.Status404NotFound, "User not found"));
            }

            // постобработка на игнорирование пустых параметров при апдейте записи
            //user.IgnorEmptyParam(existing);

            await repo.UpdateUserAsync(user.UserId, user);
            return new JsonResult(new { code = 200, message = "success" }) { StatusCode = 200 };
        }

        // PUT: api/Users/UpdateProfileUser/[id] - обновление данных профиля пользователя
        [HttpPut]
        [Route("UpdateProfileUser/{id}")]
        [ProducesResponseType(204, Type = typeof(User))]
        [ProducesResponseType(400, Type = typeof(CustomResponseExamplesBadRequest))]
        [ProducesResponseType(404, Type = typeof(CustomResponseExamplesNotFound))]
        public async Task<IActionResult> UpdateProfileUser(string id, [Required] string firstName, [Required] string surname, string? patronymic,
        DateTime? birthDate, string? mobilePhone, string? address)
        {
            // ТУТ ПОКА НЕ ВАЛИДИРУЮ ЗАПРОС, Т.К. ВАЛИДАЦИЮ СДЕЛАЛ ТОЛЬКО ПОД САМ ЭКЗЕМПЛЯР USER

            if (id is null || firstName is null || surname is null)
            {
                return BadRequest(new CustomResponseExamplesBadRequest(StatusCodes.Status400BadRequest, "userId, firstName and surname should not be empty"));
            }


            User? existing = await repo.RetrieveAsync(id);

            if (existing is null)
            {
                return NotFound(new CustomResponseExamplesNotFound(StatusCodes.Status404NotFound, "User not found"));
            }


            await repo.UpdateProfileUserAsync(id, firstName, surname, patronymic, birthDate, mobilePhone, address);
            return new NoContentResult();
        }

        // PUT: api/Users/UpdateEmail/[id] - обновление email пользователя
        [HttpPut]
        [Route("UpdateEmail/{id}")]
        [ProducesResponseType(204, Type = typeof(User))]
        [ProducesResponseType(400, Type = typeof(CustomResponseExamplesBadRequest))]
        [ProducesResponseType(404, Type = typeof(CustomResponseExamplesNotFound))]
        public async Task<IActionResult> UpdateEmail(string id, [Required] string newEmail)
        {
            // ТУТ ПОКА НЕ ВАЛИДИРУЮ ЗАПРОС, Т.К. ВАЛИДАЦИЮ СДЕЛАЛ ТОЛЬКО ПОД САМ ЭКЗЕМПЛЯР USER

            if (id is null || newEmail is null)
            {
                return BadRequest(new CustomResponseExamplesBadRequest(StatusCodes.Status400BadRequest, "id and email should not be empty"));
            }

            User? existing = await repo.RetrieveAsync(id);

            if (existing is null)
            {
                return NotFound(new CustomResponseExamplesNotFound(StatusCodes.Status404NotFound, "User not found"));
            }

            await repo.UpdateEmailAsync(id, newEmail);
            return new NoContentResult();
        }

        // DELETE: api/Users/DeletUser/[id]
        [HttpDelete("DeleteUser/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400, Type = typeof(CustomResponseExamplesBadRequest))]
        [ProducesResponseType(404, Type = typeof(CustomResponseExamplesNotFound))]
        public async Task<IActionResult> DeleteCustomer(string id)
        {
            User? existing = await repo.RetrieveAsync(id);

            if (existing is null)
            {
                return NotFound(new CustomResponseExamplesNotFound(StatusCodes.Status404NotFound, "User not found"));
            }

            bool? deleted = await repo.DeleteAsync(id);
            if (deleted.HasValue && deleted.Value)
            {
                return new NoContentResult();
            }

            return BadRequest(new CustomResponseExamplesBadRequest(StatusCodes.Status400BadRequest, $"User {id} was found but failed to delete"));
        }
    }
}
