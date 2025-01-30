using Application.Services;
using Domain.Constants;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Authorize(Roles = $"{Roles.USER},{Roles.TEACHER}")]
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
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateAnswer(
        [FromBody] AnswerForCreationDTO body
    )
    {
        throw new NotImplementedException();
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<QuestionDTO>>> PutAnswer(
        [FromRoute] Guid id,
        [FromBody] AnswerForPutDTO body
    )
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAnswer(
        [FromRoute] Guid id
    )
    {
        throw new NotImplementedException();
    }
}
