using Domain.DTO.Query;
using Domain.Entities;

namespace Domain.Contracts;

public interface IQuestionRepository
{
    Task<Question?> GetByIdAsync(Guid id);
    Task<Question> AddAsync(Question question);
    Task UpdateAsync(Question question);
    Task CompleteAsync();
    Task DeleteAsync(Guid id);
    Task<(IEnumerable<Question> Questions, int TotalItemCount)> GetItemsAsync(
        PaginationDTO paginationDTO,
        QuestionSearchDTO searchDTO,
        bool onlyPublic,
        string? userId
    );
    Task<(IEnumerable<Question> Questions, int TotalItemCount)> GetTeacherQuestionsAsync(
        PaginationDTO paginationDTO,
        Guid subjectId,
        User teacher
    );
}
