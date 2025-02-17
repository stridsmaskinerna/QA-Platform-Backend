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

[ApiController]
[Route("api/subjects")]
[Produces("application/json")]
public class SubjectController : ControllerBase
{
    private readonly IServiceManager _sm;

    public SubjectController(IServiceManager sm) => _sm = sm;

    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<SubjectDTO>>> GetSubjectList()
    {
        var subjects = await _sm.SubjectService.GetAllAsync();
        return Ok(subjects);
    }

    [HttpGet("teacher")]
    [Authorize(Roles = DomainRoles.TEACHER)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<SubjectDTO>>> GetTeacherSubjectList()
    {
        var subjects = await _sm.SubjectService.GetTeacherSubjectsAsync();
        return Ok(subjects);
    }

    // TODO. FIX BUG: Apply subject filter in repository
    [HttpGet("{id}/questions")]
    [Authorize(Roles = DomainRoles.TEACHER)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [SwaggerOperationFilter(typeof(CustomHeadersOperationFilter))]
    public async Task<ActionResult<IEnumerable<QuestionDTO>>> GetTeacherSubjectList(
        [FromRoute] Guid id,
        [FromQuery] PaginationDTO paginationDTO
    )
    {
        var (questions, totalItemCount) = await _sm.QuestionService.GetTeacherQuestionsAsync(
            paginationDTO, id);

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

        return Ok(questions);
    }

    [HttpPost]
    //[Authorize(Roles = $"{DomainRoles.ADMIN}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<SubjectDTO>> CreateNewSubject([FromBody] SubjectForCreationDTO newSubject)
    {

        var subjectDTO = await _sm.SubjectService.AddAsync(newSubject);
        return Created(String.Empty, subjectDTO);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = $"{DomainRoles.ADMIN}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSubjectAsync([FromRoute] Guid id)
    {
        await _sm.SubjectService.DeleteAsync(id);
        return Ok();
    }

    [HttpPut("{id}")]
    [Authorize(Roles = $"{DomainRoles.ADMIN}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ModifySubject([FromRoute] Guid id, [FromBody] SubjectForCreationDTO body)
    {
        await _sm.SubjectService.UpdateAsync(id, body);
        return Ok();
    }
}
