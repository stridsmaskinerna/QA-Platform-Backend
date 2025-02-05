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
    public async Task<ActionResult<IEnumerable<QuestionDTO>>> GetQuestions(
        [FromQuery] PaginationDTO paginationDTO,
        [FromQuery] QuestionSearchDTO searchDTO
    )
    {
        var (questions, totalItemCount) = await _sm.QuestionService.GetItemsAsync(
            paginationDTO, searchDTO, onlyPublic: false);
        var questionDTOList = _sm.Mapper.Map<IEnumerable<QuestionDTO>>(questions);

        var paginationMeta = new PaginationMetaDTO()
        {
            PageNr = paginationDTO.PageNr,
            Limit = paginationDTO.Limit,
            TotalItemCount = totalItemCount,
        };

        Response.Headers.Append(
            CustomHeaders.Pagination,
            JsonSerializer.Serialize(paginationMeta)
        );

        return Ok(questionDTOList);
    }

    [AllowAnonymous]
    [HttpGet("public")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<QuestionDTO>>> GetPublicQuestions(
        [FromQuery] PaginationDTO paginationDTO,
        [FromQuery] QuestionSearchDTO searchDTO
    )
    {
        var (publicQuestions, totalItemCount) = await _sm.QuestionService.GetItemsAsync(paginationDTO, searchDTO);
        var publicQuestionDTOList = _sm.Mapper.Map<IEnumerable<QuestionDTO>>(publicQuestions);

        var paginationMeta = new PaginationMetaDTO()
        {
            PageNr = paginationDTO.PageNr,
            Limit = paginationDTO.Limit,
            TotalItemCount = totalItemCount,
        };

        Response.Headers.Append(
            CustomHeaders.Pagination,
            JsonSerializer.Serialize(paginationMeta)
        );

        return Ok(publicQuestionDTOList);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<QuestionDetailedDTO>>> GetQuestionDetail(
        [FromRoute] Guid id
    )
    {
        var question = await _sm.QuestionService.GetByIdAsync(id);
        return Ok(question);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateQuestion(
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
    public async Task<ActionResult<IEnumerable<QuestionDTO>>> PutQuestion(
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
    public async Task<ActionResult<IEnumerable<QuestionDTO>>> VoteQuestion(
        [FromRoute] Guid answerId,
        [FromQuery] string vote
    )
    {
        bool? voteAsBoolean = _sm.VoteService.GetVoteAsBoolean(vote);
        await _sm.VoteService.CastVoteAsync(answerId, voteAsBoolean);
        return Ok();
    }
}
