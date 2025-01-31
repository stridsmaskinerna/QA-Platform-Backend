using Domain.Entities;

namespace Application.Services;

public interface IQuestionService
{
    Task<Question?> GetByIdAsync(Guid id);
    Task<IEnumerable<Question>> GetAllAsync(int? limit, string? searchString, bool onlyPublic = true);
    Task<Question> AddAsync(Question question);
    Task UpdateAsync(Question question);
    Task DeleteAsync(Guid id);
}
