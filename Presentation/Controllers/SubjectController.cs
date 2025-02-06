using Application.Contracts;
using Domain.DTO.Response;
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
        public async Task<IEnumerable<SubjectDTO>> GetSubjectList() {

            return await _sm.SubjectService.GetAllAsync();
        }
    }
}
