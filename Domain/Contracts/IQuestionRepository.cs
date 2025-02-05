using Domain.DTO.Query;
using Domain.Entities;

namespace Infrastructure.Repositories;

public interface IQuestionRepository
{
    Task<Question?> GetByIdAsync(Guid id);
    Task<Question> AddAsync(Question question);
    Task UpdateAsync(Question question);
    Task DeleteAsync(Guid id);

    Task<(IEnumerable<Question> Questions, int TotalItemCount)> GetItemsAsync(
        PaginationDTO paginationDTO,
        QuestionSearchDTO searchDTO,
        bool onlyPublic,
        string? userId
    );
    Task CompleteAsync();
}
