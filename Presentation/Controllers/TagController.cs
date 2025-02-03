using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using Application.Services;
using Domain.Constants;
using Domain.DTO.Header;
using Domain.DTO.Query;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Presentation.Controllers;

[ApiController]
[Route("api/tags")]
[Produces("application/json")]
public class TagController : ControllerBase
{
    private readonly IServiceManager _sm;

    public TagController(IServiceManager sM) {
        _sm = sM;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TagStandardDTO>>> GetAllTagsAsync() {

        var tagsLyst = await _sm.TagService.GetAllAsync();
        var dtoList = _sm.Mapper.Map<IEnumerable<TagStandardDTO>>(tagsLyst);
        return Ok(dtoList);
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTagAsync(Guid id)
    {
        Tag? tagToRemove = await _sm.TagService.GetByIdAsync(id);
        if ( tagToRemove == null)
        {
            return NotFound(new { message = "Tag not in the database"});
        }
        await _sm.TagService.DeleteAsync(id);
        return Ok(new { message = "Tag removed: ", data = id });
    }

}
