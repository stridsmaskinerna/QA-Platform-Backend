using Domain.DTO.Query;
using Domain.Entities;

namespace Application.Services;

public interface IQuestionService
{
    Task<Question?> GetByIdAsync(Guid id);
    Task<IEnumerable<Question>> GetAllAsync(
        PaginationDTO paginationDTO,
        QuestionSearchDTO searchDTO);
    Task<Question> AddAsync(Question question);
    Task UpdateAsync(Question question);
    Task DeleteAsync(Guid id);
}
