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

[ApiController]
[Route("api/subject")]
[Produces("application/json")]
public class SubjectController : ControllerBase
{
    private readonly IServiceManager _sm;

    public SubjectController(IServiceManager sm) => _sm = sm;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    //[Authorize]
    public async Task<ActionResult<IEnumerable<SubjectDTO>>> GetSubjectList()
    {
        var subjects = await _sm.SubjectService.GetAllAsync();
        return Ok(subjects);
    }

    [HttpGet("{id}/questions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize(Roles = DomainRoles.TEACHER)]
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

    // TODO. Do not use "create" only controller endpoint /api/subjects
    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize(Roles = $"{DomainRoles.ADMIN}")]
    public async Task<ActionResult<SubjectDTO>> CreateNewSubject([FromBody] SubjectForCreationDTO newSubject)
    {

        var subjectDTO = await _sm.SubjectService.AddAsync(newSubject);
        return Created(String.Empty, subjectDTO);
    }

    // TODO. Do not use "delete/{id}" only "{id}"
    [HttpDelete("delete/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = $"{DomainRoles.ADMIN}")]
    public async Task<IActionResult> DeleteSubjectAsync([FromRoute] Guid id)
    {
        await _sm.SubjectService.DeleteAsync(id);
        return Ok();
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = $"{DomainRoles.ADMIN}")]
    public async Task<IActionResult> ModifySubject([FromRoute] Guid id, [FromBody] SubjectForCreationDTO body)
    {
        await _sm.SubjectService.UpdateAsync(id, body);
        return Ok();
    }
}
