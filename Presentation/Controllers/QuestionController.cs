using Application.Services;
using Domain.Constants;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Presentation.Controllers;

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
        [FromQuery] int? limit,
        [FromQuery] string? searchString,
        [FromQuery] Guid? subjectId,
        [FromQuery] Guid? topicId,
        [FromQuery] string? resolvedFilter

    )
    {
        var questions = await _sm.QuestionService.GetAllAsync(limit, searchString, subjectId, topicId, resolvedFilter, onlyPublic: false);
        var questionDTOList = _sm.Mapper.Map<IEnumerable<QuestionDTO>>(questions);
        return Ok(questionDTOList);
    }

    [HttpGet("public")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<QuestionDTO>>> GetPublicQuestions(
        [FromQuery] int? limit,
        [FromQuery] string? searchString,
        [FromQuery] Guid? subjecId,
        [FromQuery] Guid? topicId,
        [FromQuery] string? resolveFilter
    )
    {
        var publicQuestions = await _sm.QuestionService.GetAllAsync(limit, searchString, subjecId, topicId, resolveFilter);
        var publicQuestionDTOList = _sm.Mapper.Map<IEnumerable<QuestionDTO>>(publicQuestions);

        return Ok(publicQuestionDTOList);

    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<QuestionDetailedDTO>>> GetQuestionDetail(
        [FromRoute] Guid id
    )
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateQuestion(
        [FromBody] QuestionForCreationDTO body
    )
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteQuestion(
        [FromRoute] Guid id
    )
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }


    [HttpPut("{id}/rating")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<QuestionDTO>>> VoteQuestion(
        [FromRoute] Guid id,
        [FromQuery] bool vote
    )
    {
        throw new NotImplementedException(); ;
    }
}
