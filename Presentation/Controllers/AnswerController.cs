using Application.Services;
using Domain.Constants;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Authorize(Roles = $"{Roles.USER}")]
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

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAnswerById(int id)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateAnswer(
        [FromBody] AnswerForCreationDTO body
    )
    {
        var created = await _sm.AnswerService.AddAsync(body);

        return CreatedAtAction(
            nameof(GetAnswerById),
            new { id = created.Id },
            created
        );
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
}
