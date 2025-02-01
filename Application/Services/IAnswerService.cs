using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;

namespace Application.Services;

public interface IAnswerService
{
    Task<AnswerDTO> AddAsync(AnswerForCreationDTO answerDTO);
    Task DeleteAsync(Guid id);
    Task<Answer?> GetByIdAsync(Guid id);
    Task UpdateAsync(Guid id, AnswerForPutDTO answerDTO);
}
