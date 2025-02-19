using Domain.DTO.Request;
using Domain.DTO.Response;

namespace Application.Contracts
{
    public interface ICommentService
    {
        Task<CommentDTO> AddAsync(CommentForCreationDTO answerDTO);
        Task DeleteAsync(Guid id);
        Task UpdateAsync(Guid id, CommentForPutDTO answerDTO);
    }
}
