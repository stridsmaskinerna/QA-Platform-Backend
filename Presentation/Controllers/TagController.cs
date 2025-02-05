using System.Collections.Generic;
using System.Data.SqlTypes;
using Application.Contracts;
using Domain.Constants;
using Domain.DTO.Response;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace Presentation.Controllers;

[ApiController]
[Route("api/tags")]
[Produces("application/json")]
public class TagController : ControllerBase
{
    private readonly IServiceManager _sm;

    public TagController(IServiceManager sM)
    {
        _sm = sM;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TagStandardDTO>>> GetFilteredTagListByValue([FromQuery] string? subTagValue)
    {
        if (string.IsNullOrWhiteSpace(subTagValue)) return Ok(_sm.Mapper.Map<IEnumerable<TagStandardDTO>>(await _sm.TagService.GetAllAsync()));

        var tagsLyst = await _sm.TagService.GetFilteredList(subTagValue);
        var dtoList = _sm.Mapper.Map<IEnumerable<TagStandardDTO>>(tagsLyst);

        return Ok(dtoList);

    }

    [HttpDelete]
    [Authorize(Roles = $"{Roles.ADMIN}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTagAsync(Guid id)
    {
        Tag? tagToRemove = await _sm.TagService.GetByIdAsync(id);
        if (tagToRemove == null)
        {
            return NotFound(new { message = "Tag not in the database" });
        }
        await _sm.TagService.DeleteAsync(id);
        return Ok(new { message = "Tag removed: ", data = id });
    }

}
