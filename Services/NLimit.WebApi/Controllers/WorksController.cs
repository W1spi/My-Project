using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLimit.WebApi.Repositoires;
using Data.NLimit.Common.EntitiesModels.SqlServer;
using System.Net;
using System.ComponentModel.DataAnnotations;
using NLimit.WebApi.Repositoires.Users;
using NLimit.WebApi.Repositoires.Works;
using Microsoft.IdentityModel.Tokens;
using NLimit.WebApi.Services.Middleware;
using Microsoft.AspNetCore.Authorization;
using FluentValidation;
using NLimit.WebApi.Services.ResponseTemplates;

namespace NLimit.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
//[Authorize]
public class WorksController : ControllerBase
{
    private readonly IWorkRepository repo;
    private readonly IValidator<Work> validator;

    public WorksController(IWorkRepository repo, IValidator<Work> validator)
    {
        this.repo = repo;
        this.validator = validator;
    }

    [HttpGet("GetAllWorks")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Work>))]
    public async Task<IEnumerable<Work>> GetWorks(string? userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return await repo.RetrieveAllAsync();
        }

        return (await repo.RetrieveAllAsync())
            .Where(w => w.UserId == userId);
    }

    [HttpGet("GetInfoAboutWork/{id}", Name = nameof(GetWorks))]
    [ProducesResponseType(200, Type = typeof(Work))]
    [ProducesResponseType(400, Type = typeof(CustomResponseExamplesBadRequest))]
    [ProducesResponseType(404, Type = typeof(CustomResponseExamplesNotFound))]
    public async Task<IActionResult> GetWork(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest(new CustomResponseExamplesBadRequest(StatusCodes.Status400BadRequest, "id must not be empty or null"));
        }

        Work? work = await repo.RetrieveAsync(id);

        if (work is null)
        {
            return NotFound(new CustomResponseExamplesNotFound(StatusCodes.Status404NotFound, "Work not found"));
        }

        return Ok(work);
    }

    [HttpPost("CreateWork", Name = nameof(GetWork))]
    [ProducesResponseType(201, Type = typeof(Work))]
    [ProducesResponseType(400, Type = typeof(CustomResponseExamplesBadRequest))]
    public async Task<IActionResult> CreateWork([FromBody] Work work)
    {
        if (work is null)
        {
            return BadRequest(new CustomResponseExamplesBadRequest(StatusCodes.Status400BadRequest, "The request body should not be empty"));
        }

        var validationResult = await validator.ValidateAsync(work);
        if (!validationResult.IsValid)
        {
            // берет первую ошибку валидации
            var query = (from errors in validationResult.Errors
                         select errors.ErrorMessage)
                        .First();

            return BadRequest(new CustomResponseExamplesBadRequest(StatusCodes.Status400BadRequest, query));
        }

        Work? addedWork = await repo.CreateAsync(work);

        if (addedWork is null)
        {
            return BadRequest(new CustomResponseExamplesBadRequest(StatusCodes.Status400BadRequest, "Repository failed to create Work"));
        }

        return CreatedAtRoute(
            routeName: nameof(GetWorks),
            routeValues: new
            {
                id = addedWork.WorkId
            },
            value: addedWork);
    }

    [HttpPut("UpdateWork")]
    [ProducesResponseType(200, Type = typeof(CustomResponseExamplesOk))]
    [ProducesResponseType(400, Type = typeof(CustomResponseExamplesBadRequest))]
    [ProducesResponseType(404, Type = typeof(CustomResponseExamplesNotFound))]
    public async Task<IActionResult> UpdateWork([FromBody] Work work)
    {
        if (work is null || work.WorkId is null)
        {
            return BadRequest(new CustomResponseExamplesBadRequest(StatusCodes.Status400BadRequest, "The request body must not be empty, and the id must also match"));
        }

        var validationResult = await validator.ValidateAsync(work);
        if (!validationResult.IsValid)
        {
            // берет первую ошибку валидации
            var query = (from errors in validationResult.Errors
                         select errors.ErrorMessage)
                        .First();

            return BadRequest(new CustomResponseExamplesBadRequest(StatusCodes.Status400BadRequest, query));
        }

        Work? existWork = await repo.RetrieveAsync(work.WorkId);

        if (existWork is null)
        {
            return NotFound(new CustomResponseExamplesNotFound(StatusCodes.Status404NotFound, "Work not found"));
        }

        await repo.UpdateAsync(work.WorkId, work);
        return Ok(new CustomResponseExamplesOk(StatusCodes.Status200OK, "success"));
    }
    
    [HttpDelete("DeleteWork/{id}")]
    [ProducesResponseType(200, Type = typeof(CustomResponseExamplesOk))]
    [ProducesResponseType(400, Type = typeof(CustomResponseExamplesBadRequest))]
    [ProducesResponseType(404, Type = typeof(CustomResponseExamplesNotFound))]
    public async Task<IActionResult> DeleteWork(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest(new CustomResponseExamplesBadRequest(StatusCodes.Status400BadRequest, "id must not be empty or null"));
        }

        Work? existWork = await repo.RetrieveAsync(id);
        if (existWork is null)
        {
            return NotFound(new CustomResponseExamplesNotFound(StatusCodes.Status404NotFound, "Work not found"));
        }

        bool? deleted = await repo.DeleteAsync(id);
        if (deleted.HasValue && deleted.Value)
        {
            return Ok(new CustomResponseExamplesOk(StatusCodes.Status200OK, "success"));
        }

        return BadRequest(new CustomResponseExamplesBadRequest(StatusCodes.Status400BadRequest, $"Work '{id}' was found but failed to delete"));
    }
}