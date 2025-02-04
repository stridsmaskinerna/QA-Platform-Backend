using Application.Contracts;
using Domain.Constants;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Authorize(Roles = $"{Roles.USER},{Roles.TEACHER}")]
[ApiController]
[Route("api/comments")]
[Produces("application/json")]
public class CommentController : ControllerBase
{
    private readonly IServiceManager _sm;

    public CommentController(IServiceManager sm)
    {
        _sm = sm;
    }


    [HttpPost]
    [Authorize(Roles = Roles.USER)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateComment(
        [FromBody] CommentForCreationDTO body
    )
    {
        var created = await _sm.CommentService.AddAsync(body);
        return Created(string.Empty, created);
    }


    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<QuestionDTO>>> PutComment(
        [FromRoute] Guid id,
        [FromBody] CommentForPutDTO body
    )
    {
        await _sm.CommentService.UpdateAsync(id, body);
        return Ok();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteComment(
        [FromRoute] Guid id
    )
    {
        await _sm.CommentService.DeleteAsync(id);
        return Ok();
    }
}
