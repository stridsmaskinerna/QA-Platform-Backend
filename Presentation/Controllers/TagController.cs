using Application.Contracts;
using Domain.Constants;
using Domain.DTO.Response;
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
    public async Task<ActionResult<IEnumerable<TagStandardDTO>>> GetFilteredTagListByValue(
        [FromQuery] string? subTagValue
    )
    {
        if (string.IsNullOrWhiteSpace(subTagValue))
        {
            return Ok(await _sm.TagService.GetAllAsync());
        }

        var tags = await _sm.TagService.GetFilteredList(subTagValue);

        return Ok(tags);

    }

    [HttpDelete]
    [Authorize(Roles = $"{DomainRoles.ADMIN}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTagAsync(Guid id)
    {
        var tagToRemove = await _sm.TagService.GetByIdAsync(id);
        await _sm.TagService.DeleteAsync(id);
        return Ok(new { message = "Tag removed: ", data = id });
    }

}
