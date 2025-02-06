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
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[Authorize(Roles = $"{DomainRoles.ADMIN}")]
        public async Task<IEnumerable<SubjectDTO>> GetSubjectList() {

            return await _sm.SubjectService.GetAllAsync();
        }

        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task CreateNewSubject([FromBody] SubjectForCreationDTO newSubject) {

            await _sm.SubjectService.AddAsync(newSubject);

        }
    }
}
