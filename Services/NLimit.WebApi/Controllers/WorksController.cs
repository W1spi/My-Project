using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLimit.WebApi.Repositoires;
using Data.NLimit.Common.EntitiesModels.SqlServer;
using System.Net;
using System.ComponentModel.DataAnnotations;
using NLimit.WebApi.Repositoires.Users;
using NLimit.WebApi.Repositoires.Works;
using Microsoft.IdentityModel.Tokens;

namespace NLimit.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WorksController : ControllerBase
{
    private readonly IWorkRepository repo;

    public WorksController(IWorkRepository repo)
    {
        this.repo = repo;
    }

    [HttpGet]
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
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetWork(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest("Номер задания не передан!");
        }

        Work? work = await repo.RetrieveAsync(id);

        if (work is null)
        {
            return NotFound("Задание не найдено!");
        }

        return Ok(work);
    }

    [HttpPost("CreateWork", Name = nameof(GetWork))]
    [ProducesResponseType(201, Type = typeof(Work))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateWork([FromBody] Work work)
    {
        if (work is null)
        {
            return BadRequest();
        }

        Work? addedWork = await repo.CreateAsync(work);

        if (addedWork is null)
        {
            return BadRequest("Repository failed to create Work.");
        }

        return CreatedAtRoute(
            routeName: nameof(GetWorks),
            routeValues: new
            {
                id = addedWork.WorkId
            },
            value: addedWork);
    }

    [HttpPut("UpdateWork/{id}")]
    [ProducesResponseType(204, Type = typeof(Work))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateWork(string id, [FromBody] Work work)
    {
        if (work is null)
        {
            return BadRequest();
        }

        Work? existWork = await repo.RetrieveAsync(id);

        if (existWork is null)
        {
            return NotFound();
        }

        await repo.UpdateAsync(id, work);
        return new NoContentResult();
    }
    
    [HttpDelete("DeleteWork/{id}")]
    [ProducesResponseType(204, Type = typeof(Work))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> DeleteWork(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest();
        }

        Work? existWork = await repo.RetrieveAsync(id);
        if (existWork is null)
        {
            return NotFound();
        }

        bool? deleted = await repo.DeleteAsync(id);
        if (deleted.HasValue && deleted.Value)
        {
            return new NoContentResult();
        }
        else
        {
            return BadRequest();
        }
    }
}