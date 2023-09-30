using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Data.NLimit.Common.EntitiesModels.SqlServer;
using System.Net;
using System.ComponentModel.DataAnnotations;
using NLimit.WebApi.Repositoires.Users;
using static NLimit.WebApi.Services.UserProcessingRequestService;
using NLimit.WebApi.Services;

namespace NLimit.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository repo;


        public UsersController (IUserRepository repo)
        {
            this.repo = repo;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<User>))]
        public async Task<IEnumerable<User>> GetUsers (string? firstName)
        {
            if (string.IsNullOrEmpty(firstName)) 
            {
                return await repo.RetrieveAllAsync();
            }

            return (await repo.RetrieveAllAsync())
                    .Where(s => s.FirstName == firstName);
        }

        [HttpGet("{id}", Name = nameof(GetUsers))]
        [ProducesResponseType(200, Type = typeof(User))]
        [ProducesResponseType(404, Type = typeof(CustomServiceResponseNotFound))]
        public async Task<IActionResult> GetUser(string id)
        {
            User? user = await repo.RetrieveAsync(id);

            // блок с приведением UserId к нижнему/верхнему регистру
            /*if (user is null)
            {
                id = id.ToUpper();
                user = await repo.RetrieveAsync(id);

                if (user is null)
                {
                    id = id.ToLower();
                    user = await repo.RetrieveAsync(id);

                    if (user is null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        return Ok(user);
                    }
                }
                else
                {
                    return Ok(user);
                }
            }
            else
            {
                return Ok(user);
            }*/

            if (user is null)
            {
                return NotFound(new CustomServiceResponseNotFound(StatusCodes.Status404NotFound, "Not Found"));
            }
            return Ok(user);
        }

        [HttpPost(Name = nameof(GetUser))]
        [ProducesResponseType(201, Type = typeof(User))]
        [ProducesResponseType(400, Type = typeof(CustomServiceResponseBadRequest))]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            if (user is null)
            {
                return BadRequest(new CustomServiceResponseBadRequest(StatusCodes.Status400BadRequest, "Bad Request"));
            }

            user = ProcessingRequestParameters(user, UserOperationType.Create);

            User? addedUser = await repo.CreateAsync(user);
            if (addedUser is null)
            {
                return BadRequest("Repository failed to create User.");
            }

            return CreatedAtRoute(
                routeName: nameof(GetUsers),
                routeValues: new
                {
                    id = addedUser.UserId.ToLower()
                },
                value: addedUser);
        }

        // PUT: api/Users/UpdateUser/[id] - полное обновление пользователя
        [HttpPut]
        [Route("UpdateUser/{id}")]
        [ProducesResponseType(204, Type = typeof(User))]
        [ProducesResponseType(400, Type = typeof(CustomServiceResponseBadRequest))]
        [ProducesResponseType(404, Type = typeof(CustomServiceResponseNotFound))]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] User user)
        {
            if (user is null || id != user.UserId)
            {
                return BadRequest(new CustomServiceResponseBadRequest(StatusCodes.Status400BadRequest, "Bad Request"));
            }

            User? existing = await repo.RetrieveAsync(id);

            // блок с приведением UserId к нижнему/верхнему регистру
            /*if (existing is null) 
            {
                id = id.ToUpper();
                user.UserId = user.UserId.ToUpper();

                existing = await repo.RetrieveAsync(id);

                if (existing is null)
                {
                    id = id.ToLower();
                    user.UserId = user.UserId.ToLower();

                    existing = await repo.RetrieveAsync(id);

                    if (existing is null)
                    {
                        return NotFound();
                    }

                    await repo.UpdateUserAsync(id, user);
                    return new NoContentResult();
                }

                await repo.UpdateUserAsync(id, user);
                return new NoContentResult();
            }*/

            if (existing is null)
            {
                return NotFound(new CustomServiceResponseNotFound(StatusCodes.Status404NotFound, "Not Found"));
            }

            await repo.UpdateUserAsync(id, user);
            return new NoContentResult();
        }

        // PUT: api/Users/UpdateProfileUser/[id] - обновление данных профиля пользователя
        [HttpPut]
        [Route("UpdateProfileUser/{id}")]
        [ProducesResponseType(204, Type = typeof(User))]
        [ProducesResponseType(400, Type = typeof(CustomServiceResponseBadRequest))]
        [ProducesResponseType(404, Type = typeof(CustomServiceResponseNotFound))]
        public async Task<IActionResult> UpdateProfileUser(string id, [Required] string firstName, [Required] string surname, string? patronymic,
        DateTime? birthDate, string? mobilePhone, string? address)
        {
            if (id is null || firstName is null || surname is null)
            {
                return BadRequest(new CustomServiceResponseBadRequest(StatusCodes.Status400BadRequest, "Bad Request"));
            }

            User? existing = await repo.RetrieveAsync(id);

            // блок с приведением UserId к нижнему/верхнему регистру
            /*if (existing is null)
            {
                id = id.ToUpper();
                existing = await repo.RetrieveAsync(id);

                if (existing is null)
                {
                    id = id.ToLower();
                    existing = await repo.RetrieveAsync(id);

                    if (existing is null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        await repo.UpdateProfileUserAsync(id, firstName, surname, patronymic, birthDate, mobilePhone, address);
                        return new NoContentResult();
                    }
                }
                else
                {
                    await repo.UpdateProfileUserAsync(id, firstName, surname, patronymic, birthDate, mobilePhone, address);
                    return new NoContentResult();
                }
            }
            else
            {
                await repo.UpdateProfileUserAsync(id, firstName, surname, patronymic, birthDate, mobilePhone, address);
                return new NoContentResult();
            }*/

            if (existing is null)
            {
                return NotFound(new CustomServiceResponseNotFound(StatusCodes.Status404NotFound, "Not Found"));
            }

            await repo.UpdateProfileUserAsync(id, firstName, surname, patronymic, birthDate, mobilePhone, address);
            return new NoContentResult();
        }

        // PUT: api/Users/UpdateEmail/[id] - обновление email пользователя
        [HttpPut]
        [Route("UpdateEmail/{id}")]
        [ProducesResponseType(204, Type = typeof(User))]
        [ProducesResponseType(400, Type = typeof(CustomServiceResponseBadRequest))]
        [ProducesResponseType(404, Type = typeof(CustomServiceResponseNotFound))]
        public async Task<IActionResult> UpdateEmail(string id, [Required] string newEmail)
        {
            if (id is null || newEmail is null)
            {
                return BadRequest(new CustomServiceResponseBadRequest(StatusCodes.Status400BadRequest, "Bad Request"));
            }

            User? existing = await repo.RetrieveAsync(id);
            // блок с приведением UserId к нижнему/верхнему регистру
            /*if (existing is null)
            {
                id = id.ToUpper();
                existing = await repo.RetrieveAsync(id);

                if (existing is null)
                {
                    id = id.ToLower();
                    existing = await repo.RetrieveAsync(id);

                    if (existing is null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        await repo.UpdateEmailAsync(id, newEmail);
                        return new NoContentResult();
                    }
                }
                else
                {
                    await repo.UpdateEmailAsync(id, newEmail);
                    return new NoContentResult();
                }
            }
            else
            {
                await repo.UpdateEmailAsync(id, newEmail);
                return new NoContentResult();
            }*/

            if (existing is null)
            {
                return NotFound(new CustomServiceResponseNotFound(StatusCodes.Status404NotFound, "Not Found"));
            }

            await repo.UpdateEmailAsync(id, newEmail);
            return new NoContentResult();
        }

        // DELETE: api/Users/[id]
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400, Type = typeof(CustomServiceResponseBadRequest))]
        [ProducesResponseType(404, Type = typeof(CustomServiceResponseNotFound))]
        public async Task<IActionResult> DeleteCustomer(string id)
        {
            User? existing = await repo.RetrieveAsync(id);

            // блок с приведением UserId к нижнему/верхнему регистру
            /*if (existing is null)
            {
                id = id.ToUpper();
                existing = await repo.RetrieveAsync(id);

                if (existing is null)
                {
                    id = id.ToLower();
                    existing = await repo.RetrieveAsync(id);

                    if (existing is null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        bool? deleted = await repo.DeleteAsync(id);
                        if (deleted.HasValue && deleted.Value)
                        {
                            return new NoContentResult();
                        }
                        else
                        {
                            return BadRequest($"User {id} was found but failed to delete.");
                        }
                    }
                }
                else
                {
                    bool? deleted = await repo.DeleteAsync(id);
                    if (deleted.HasValue && deleted.Value)
                    {
                        return new NoContentResult();
                    }
                    else
                    {
                        return BadRequest($"User {id} was found but failed to delete.");
                    }
                }
            }
            else
            {
                bool? deleted = await repo.DeleteAsync(id);
                if (deleted.HasValue && deleted.Value)
                {
                    return new NoContentResult();
                }
                else
                {
                    return BadRequest($"User {id} was found but failed to delete.");
                }
            }*/

            if (existing is null)
            {
                return NotFound(new CustomServiceResponseNotFound(StatusCodes.Status404NotFound, "Not Found"));
            }

            bool? deleted = await repo.DeleteAsync(id);
            if (deleted.HasValue && deleted.Value)
            {
                return new NoContentResult();
            }

            return BadRequest($"User {id} was found but failed to delete.");
        }
    }
}
