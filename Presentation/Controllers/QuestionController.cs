using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Infrastructure.Repositories;
using Domain.DTO;
using Domain;
using Domain.Entities;
using Application.Services;


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
            return Ok(courseDTOList) ;

        }
    }
}
