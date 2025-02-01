using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;
using Infrastructure.Repositories;

namespace Application.Services;

public class AnswerService : BaseService, IAnswerService
{
    private readonly IAnswerRepository _answerRepository;
    private readonly IServiceManager _sm;

    public AnswerService(
        IAnswerRepository answerRepository,
        IServiceManager sm
    )
    {
        _answerRepository = answerRepository;
        _sm = sm;
    }

    public async Task<Answer?> GetByIdAsync(Guid id)
    {
        var answer = await _answerRepository.GetByIdAsync(id);
        if (answer == null)
        {
            NotFound();
        }
        return answer;
    }

    public async Task<AnswerDTO> AddAsync(AnswerForCreationDTO answerDTO)
    {
        var answer = _sm.Mapper.Map<Answer>(answerDTO);
        if (answer == null)
        {
            BadRequest("Could not create the new answer");
        }

        var createdAnswer = await _answerRepository.AddAsync(answer);

        return _sm.Mapper.Map<AnswerDTO>(createdAnswer);
    }

    public async Task UpdateAsync(
        Guid id,
        AnswerForPutDTO answerDTO
    )
    {
        var answer = await _answerRepository.GetByIdAsync(id);
        if (answer == null)
        {
            NotFound($"No answer with id {id} exist.");
        }

        var updated = _sm.Mapper.Map(answerDTO, answer);

        await _answerRepository.UpdateAsync(updated);
    }

    public async Task DeleteAsync(Guid id)
    {
        var answer = await _answerRepository.GetByIdAsync(id);
        if (answer == null)
        {
            NotFound($"No answer with id {id} exist.");
        }

        await _answerRepository.DeleteAsync(answer);
    }
}
