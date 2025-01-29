using Application.Services;
using Domain.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/questions")]
[Produces("application/json")]
public class QuestionController : ControllerBase
{
    private readonly IServiceManager _sm;

    public QuestionController(IServiceManager sM)
    {
        _sm = sM;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<QuestionDTO>>> GetPublicQuestionsList(
        [FromQuery] string? searchString,
        [FromQuery] int? limit
    )
    {
        var publicQuestion = await _sm.QuestionService.GetAllAsync();
        var courseDTOList = _sm.Mapper.Map<IEnumerable<QuestionDTO>>(publicQuestion);
        return Ok(courseDTOList);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IEnumerable<QuestionDetailedDTO>>> GetQuestionDetail(
        [FromRoute] Guid id
    )
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public async Task<IActionResult> CreateQuestion(
        [FromBody] QuestionForCreationDTO body
    )
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQuestion(
        [FromRoute] Guid id
    )
    {
        throw new NotImplementedException();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<IEnumerable<QuestionDTO>>> PutQuestion(
        [FromRoute] Guid id,
        [FromBody] QuestionForPutDTO body
    )
    {
        throw new NotImplementedException();
    }


    [HttpPut("{id}/rating")]
    public async Task<ActionResult<IEnumerable<QuestionDTO>>> VoteQuestion(
        [FromRoute] Guid id,
        [FromQuery] bool vote
    )
    {
        throw new NotImplementedException(); ;
    }
}
