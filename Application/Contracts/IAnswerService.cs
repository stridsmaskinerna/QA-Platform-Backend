using Domain.DTO.Request;
using Domain.DTO.Response;

namespace Application.Contracts;

public interface IAnswerService
{
    Task<AnswerDTO> AddAsync(AnswerForCreationDTO answerDTO);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<CommentDTO>> GetAnswerCommentsAsync(Guid id);
    Task ManageVisibilityAsync(Guid id);
    Task UpdateAsync(Guid id, AnswerForPutDTO answerDTO);
    Task ToggleAccepted(Guid id);
}
