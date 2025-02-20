using System.Text.Json;
using Application.Contracts;
using Domain.Constants;
using Domain.DTO.Header;
using Domain.DTO.Query;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Filters;
using Swashbuckle.AspNetCore.Annotations;

namespace Presentation.Controllers;

[Authorize(Roles = $"{DomainRoles.USER}")]
[ApiController]
[Route("api/questions")]
[Produces("application/json")]
public class QuestionController : ControllerBase
{
    private readonly IServiceManager _sm;

    public QuestionController(IServiceManager sm)
    {
        _sm = sm;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerOperationFilter(typeof(CustomHeadersOperationFilter))]
    public async Task<ActionResult<IEnumerable<QuestionDTO>>> GetQuestions(
        [FromQuery] PaginationDTO paginationDTO,
        [FromQuery] QuestionSearchDTO searchDTO
    )
    {
        var (questions, totalItemCount) = await _sm.QuestionService.GetItemsAsync(
            paginationDTO, searchDTO);

        var paginationMeta = new PaginationMetaDTO()
        {
            PageNr = paginationDTO.PageNr,
            Limit = paginationDTO.Limit,
            TotalItemCount = totalItemCount
        };

        Response.Headers.Append(
            CustomHeaders.Pagination,
            JsonSerializer.Serialize(paginationMeta)
        );

        return Ok(questions);
    }

    [HttpGet("public")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerOperationFilter(typeof(CustomHeadersOperationFilter))]
    public async Task<ActionResult<IEnumerable<QuestionDTO>>> GetPublicQuestions(
        [FromQuery] PaginationDTO paginationDTO,
        [FromQuery] QuestionSearchDTO searchDTO
    )
    {
        var (publicQuestions, totalItemCount) = await _sm.QuestionService.GetPublicItemsAsync(paginationDTO, searchDTO);

        var paginationMeta = new PaginationMetaDTO()
        {
            PageNr = paginationDTO.PageNr,
            Limit = paginationDTO.Limit,
            TotalItemCount = totalItemCount
        };

        Response.Headers.Append(
            CustomHeaders.Pagination,
            JsonSerializer.Serialize(paginationMeta)
        );

        return Ok(publicQuestions);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<QuestionDetailedDTO>> GetQuestionDetail(
        [FromRoute] Guid id
    )
    {
        var question = await _sm.QuestionService.GetByIdAsync(id);
        return Ok(question);
    }

    [AllowAnonymous]
    [HttpGet("public/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<QuestionDetailedDTO>> GetQuestionDetailPublic(
        [FromRoute] Guid id
    )
    {
        var question = await _sm.QuestionService.GetPublicQuestionByIdAsync(id);
        return Ok(question);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<QuestionDTO>> CreateQuestion(
        [FromBody] QuestionForCreationDTO body
    )
    {
        var question = await _sm.QuestionService.AddAsync(body);
        return Created(String.Empty, question);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteQuestion(
        [FromRoute] Guid id
    )
    {
        await _sm.QuestionService.DeleteAsync(id);
        return Ok();
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutQuestion(
        [FromRoute] Guid id,
        [FromBody] QuestionForPutDTO body
    )
    {
        await _sm.QuestionService.UpdateAsync(id, body);
        return Ok();
    }


    [HttpPut("{answerId}/rating")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VoteQuestion(
        [FromRoute] Guid answerId,
        [FromQuery] string vote
    )
    {
        bool? voteAsBoolean = _sm.VoteService.GetVoteAsBoolean(vote);
        await _sm.VoteService.CastVoteAsync(answerId, voteAsBoolean);
        return Ok();
    }

    [HttpPut("{id}/visibility")]
    [Authorize(Roles = DomainRoles.TEACHER)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ManageQuestionVisibility([FromRoute] Guid id)
    {
        await _sm.QuestionService.ManageQuestionVisibilityAsync(id);
        return Ok();
    }
}
