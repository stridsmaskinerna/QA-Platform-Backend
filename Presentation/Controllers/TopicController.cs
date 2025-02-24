using Application.Contracts;
using Domain.Constants;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;


[ApiController]
[Authorize(Roles = $"{DomainRoles.TEACHER}")]
[Route("api/topics")]
[Produces("application/json")]
public class TopicController : ControllerBase
{
    private readonly IServiceManager _sm;

    public TopicController(IServiceManager sm)
    {
        _sm = sm;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TopicDTO>> CreateTopic(
        [FromBody] TopicForCreationDTO body
    )
    {
        var created = await _sm.TopicService.AddAsync(body);
        return Created(string.Empty, created);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutTopic(
        [FromRoute] Guid id,
        [FromBody] TopicDTO body
    )
    {
        await _sm.TopicService.UpdateAsync(id, body);
        return Ok();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTopic(
        [FromRoute] Guid id
    )
    {
        var deletedTopic = await _sm.TopicService.DeleteAsync(id);
        if (deletedTopic == null) return new ForbidResult(new AuthenticationProperties
        {
            RedirectUri = "Unable to delete Topic"
        }); 
        return Ok();
    }
}
