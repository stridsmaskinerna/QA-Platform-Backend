using Domain.Entities;

namespace Domain.Contracts;

public interface IAnswerRepository
{
    Task<Answer?> GetByIdAsync(Guid id);
    Task<Answer> AddAsync(Answer answer);
    Task UpdateAsync(Answer answer);
    Task DeleteAsync(Answer answer);
    Task<IEnumerable<Answer>> GetAnswersByQuestionIdAsync(Guid questionId);
    Task<IEnumerable<Answer>> GetAnswersByUserIdAsync(Guid userId);
    Task<int> GetAnswerCountByQuestionIdAsync(Guid questionId);
    Task CompleteAsync();
}
