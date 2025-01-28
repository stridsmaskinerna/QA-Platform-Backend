using Application.Services;
using AutoMapper;
using Domain;
using Domain.DTO;
using Domain.Entities;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Presentation.Controllers
{
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
        public async Task<ActionResult<IEnumerable<QuestionDTO>>> GetPublicQuestionsList()
        {
            var publicQuestion = await _sm.QuestionService.GetAllAsync();
            var courseDTOList = _sm.Mapper.Map<IEnumerable<QuestionDTO>>(publicQuestion);
            return Ok(courseDTOList);

        }
    }
}
