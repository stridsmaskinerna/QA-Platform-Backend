using Application.Contracts;
using Domain.Contracts;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;

namespace Application.Services;

public class AnswerService : BaseService, IAnswerService
{
    private readonly IRepositoryManager _rm;
    private readonly IServiceManager _sm;

    public AnswerService(
        IRepositoryManager rm,
        IServiceManager sm
    )
    {
        _rm = rm;
        _sm = sm;
    }

    public static string MsgAddAsyncBadRequest() => "Could not create the new answer";

    public static string MsgNotFound(Guid id) => $"No answer with id {id} exist.";

    public async Task<AnswerDTO> AddAsync(AnswerForCreationDTO answerDTO)
    {
        var answer = _sm.Mapper.Map<Answer>(answerDTO);
        if (answer == null)
        {
            BadRequest(MsgAddAsyncBadRequest());
        }

        answer.UserId = _sm.TokenService.GetUserId();
        var createdAnswer = await _rm.AnswerRepository.AddAsync(answer);
        var createdAnswerDTO = _sm.Mapper.Map<AnswerDTO>(createdAnswer);
        createdAnswerDTO.UserName = _sm.TokenService.GetUserName();
        return createdAnswerDTO;
    }

    public async Task UpdateAsync(
        Guid id,
        AnswerForPutDTO answerDTO
    )
    {
        var answer = await _rm.AnswerRepository.GetByIdAsync(id);
        if (answer == null)
        {
            NotFound(MsgNotFound(id));
        }

        var updated = _sm.Mapper.Map(answerDTO, answer);

        await _rm.AnswerRepository.CompleteAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var answer = await _rm.AnswerRepository.GetByIdAsync(id);
        if (answer == null)
        {
            NotFound(MsgNotFound(id));
        }

        await _rm.AnswerRepository.DeleteAsync(answer);
    }

    public async Task ManageVisibilityAsync(Guid id)
    {
        var answer = await _rm.AnswerRepository.GetByIdAsync(id);
        if (answer == null)
        {
            NotFound(MsgNotFound(id));
        }
        answer.IsHidden = !answer.IsHidden;
        await _rm.AnswerRepository.UpdateAsync(answer);
    }
}
