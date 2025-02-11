using Domain.Entities;

namespace Domain.Contracts;

public interface ICommentRepository
{
    Task<Comment?> GetByIdAsync(Guid id);
    Task<IEnumerable<Comment>> GetCommentsByAnswerIdAsync(Guid answerId);
    Task<IEnumerable<Comment>> GetCommentsByUserIdAsync(Guid userId);
    Task<Comment> AddAsync(Comment comment);
    Task UpdateAsync(Comment comment);
    Task DeleteAsync(Comment comment);
}
