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
    public async Task<ActionResult<IEnumerable<QuestionDTO>>> GetPublicQuestions(
        [FromQuery] int? limit,
        [FromQuery] string? searchString
    )
    {
        var questions = await _sm.QuestionService.GetAllAsync(limit, searchString);
        var courseDTOList = _sm.Mapper.Map<IEnumerable<QuestionDTO>>(questions);
        return Ok(courseDTOList);
    }

    [HttpGet("public")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<QuestionDTO>>> GetPublicQuestionsList(
        [FromQuery] int? limit,
        [FromQuery] string? searchString
    )
    {

        // TODO Npgsql.EntityFrameworkCore.PostgreSQL assembly EF.Functions.ILike
        // TODO! Use searching to search for question, question title, tags, subject name, subject code, topic name, etc 
        var publicQuestion = await _sm.QuestionService.GetAllAsync();

        List<QuestionDTO> questionDTOList = new();

        foreach (Question q in publicQuestion)
        {

            var extraObj = await _sm.Context.Questions
                .Select(q => new
                {
                    Question = q,
                    TopicName = _sm.Context.Topics
                            .Where(t => t.Id == q.TopicId)
                            .Select(t => t.Name)
                            .FirstOrDefault(),
                    SubjectName = _sm.Context.Subjects
                            .Where(s => s.Topics.Any(t => t.Id == q.TopicId))
                            .Select(s => s.Name)
                            .FirstOrDefault(),
                    SubjectCode = _sm.Context.Subjects
                            .Where(s => s.Topics.Any(t => t.Id == q.TopicId))
                            .Select(s => s.SubjectCode)
                            .FirstOrDefault(),
                    SubjectId = _sm.Context.Subjects
                            .Where(s => s.Topics.Any(t => t.Id == q.TopicId))
                            .Select(s => s.Id)
                            .FirstOrDefault(),
                    UserName = _sm.Context.Users
                            .Where(u => u.Id == q.UserId)
                            .Select(u => u.UserName)
                            .FirstOrDefault(),
                    T = q.Tags.Select(t => t.Value).ToList()
                })
                .FirstAsync();

            QuestionDTO dto = _sm.Mapper.Map<QuestionDTO>(q);
            dto.TopicName = extraObj.TopicName!;
            dto.SubjectName = extraObj.SubjectName!;
            dto.SubjectCode = extraObj.SubjectCode;
            dto.SubjectId = $"{extraObj.SubjectId!}";
            dto.UserName = extraObj.UserName!;
            dto.Tags = extraObj.T;


            questionDTOList.Add(dto);

        }

        return Ok(questionDTOList);

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
