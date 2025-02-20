using Application.Contracts;
using Domain.Constants;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Authorize(Roles = $"{DomainRoles.USER}")]
[ApiController]
[Route("api/answers")]
[Produces("application/json")]
public class AnswerController : ControllerBase
{
    private readonly IServiceManager _sm;

    public AnswerController(IServiceManager sm)
    {
        _sm = sm;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AnswerDTO>> CreateAnswer(
        [FromBody] AnswerForCreationDTO body
    )
    {
        var created = await _sm.AnswerService.AddAsync(body);

        return Created(string.Empty, created);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAnswer(
        [FromRoute] Guid id,
        [FromBody] AnswerForPutDTO body
    )
    {
        await _sm.AnswerService.UpdateAsync(id, body);
        return Ok();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAnswer(
        [FromRoute] Guid id
    )
    {
        await _sm.AnswerService.DeleteAsync(id);
        return Ok();
    }

    [HttpPut("{id}/visibility")]
    [Authorize(Roles = DomainRoles.TEACHER)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ManageAnswerVisibility(Guid id)
    {
        await _sm.AnswerService.ManageVisibilityAsync(id);
        return Ok();
    }

    [HttpGet("{id}/comments")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<CommentDTO>>> GetAnswerComments(
        [FromRoute] Guid id
    )
    {
        IEnumerable<CommentDTO> comments = await _sm.AnswerService.GetAnswerCommentsAsync(id);
        return Ok(comments);
    }
}
