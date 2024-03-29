﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Data.NLimit.Common.EntitiesModels.SqlServer;
using System.Net;
using System.ComponentModel.DataAnnotations;
using NLimit.WebApi.Repositoires.Users;

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
            else
            {
                return (await repo.RetrieveAllAsync())
                    .Where(s => s.FirstName == firstName);
            }
        }

        [HttpGet("{id}", Name = nameof(GetUsers))]
        [ProducesResponseType(200, Type = typeof(User))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUser(string id)
        {
            User? user = await repo.RetrieveAsync(id);

            if (user is null)
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
            }
        }

        [HttpPost(Name = nameof(GetUser))]
        [ProducesResponseType(201, Type = typeof(User))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            if (user is null)
            {
                return BadRequest();
            }

            User? addedUser = await repo.CreateAsync(user);
            if (addedUser is null)
            {
                return BadRequest("Repository failed to create User.");
            }
            else
            {
                return CreatedAtRoute(
                    routeName: nameof(GetUsers),
                    routeValues: new
                    {
                        id = addedUser.UserId.ToLower()
                    },
                    value: addedUser);
            }
        }

        // PUT: api/Users/UpdateUser/[id] - полное обновление пользователя
        [HttpPut]
        [Route("UpdateUser/{id}")]
        [ProducesResponseType(204,Type = typeof(User))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] User user)
        {
            if (user is null)
            {
                return BadRequest();
            }

            User? existing = await repo.RetrieveAsync(id);

            if (existing is null) 
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
                    else
                    {
                        await repo.UpdateUserAsync(id, user);
                        return new NoContentResult();
                    }
                }
                else
                {
                    await repo.UpdateUserAsync(id, user);
                    return new NoContentResult();
                }
            }
            else
            {
                await repo.UpdateUserAsync(id, user);
                return new NoContentResult();
            }
        }

        // PUT: api/Users/UpdateProfileUser/[id] - обновление данных профиля пользователя
        [HttpPut]
        [Route("UpdateProfileUser/{id}")]
        [ProducesResponseType(204, Type = typeof(User))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateProfileUser(string id, [Required] string firstName, [Required] string surname, string? patronymic,
        DateTime? birthDate, string? mobilePhone, string? address)
        {
            if (id is null || firstName is null || surname is null)
            {
                return BadRequest();
            }

            User? existing = await repo.RetrieveAsync(id);

            if (existing is null)
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
            }
        }

        // PUT: api/Users/UpdateEmail/[id] - обновление email пользователя
        [HttpPut]
        [Route("UpdateEmail/{id}")]
        [ProducesResponseType(204, Type = typeof(User))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateEmail(string id, [Required] string newEmail)
        {
            if (id is null || newEmail is null)
            {
                return BadRequest();
            }

            User? existing = await repo.RetrieveAsync(id);

            if (existing is null)
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
            }
        }

        // DELETE: api/Users/[id]
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteCustomer(string id)
        {
            User? existing = await repo.RetrieveAsync(id);

            if (existing is null)
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
            }
        }
    }
}
