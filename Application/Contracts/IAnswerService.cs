using Domain.DTO.Request;
using Domain.DTO.Response;

namespace Application.Contracts;

public interface IAnswerService
{
    Task<AnswerDTO> AddAsync(AnswerForCreationDTO answerDTO);
    Task DeleteAsync(Guid id);
    Task UpdateAsync(Guid id, AnswerForPutDTO answerDTO);
}
