using Application.Contracts;
using Domain.Constants;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
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
        public async Task<IEnumerable<SubjectDTO>> GetSubjectList()
        {

            return await _sm.SubjectService.GetAllAsync();
        }

        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[Authorize(Roles = $"{DomainRoles.ADMIN}")]
        public async Task<ActionResult<SubjectDTO>> CreateNewSubject([FromBody] SubjectForCreationDTO newSubject)
        {

            var subjectDTO = await _sm.SubjectService.AddAsync(newSubject);
            return Created(String.Empty, subjectDTO);
        }

        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[Authorize(Roles = $"{DomainRoles.ADMIN}")]
        public async Task DeleteSubjectAsync([FromRoute] Guid Id)
        {
            await _sm.SubjectService.DeleteAsync(Id);

        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[Authorize(Roles = $"{DomainRoles.ADMIN}")]
        public async Task<ActionResult<IEnumerable<QuestionDTO>>> ModifySubcect([FromRoute] Guid id, [FromBody] SubjectForCreationDTO body)
        {
            await _sm.SubjectService.UpdateAsync(id, body);
            return Ok();
        }
    }
}
