using Domain.Entities;

namespace Infrastructure.Repositories;

public interface IQuestionRepository
{
    Task<Question?> GetByIdAsync(Guid id);
    Task<Question> AddAsync(Question question);
    Task UpdateAsync(Question question);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<Question>> GetAllAsync(int? limit, string? searchString);
    Task<IEnumerable<Question>> GetAllPublicAsync(int? limit, string? searchString);
}
