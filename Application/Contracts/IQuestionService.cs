using Domain.DTO.Query;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;

namespace Application.Contracts;

public interface IQuestionService
{
    Task<QuestionDetailedDTO> GetByIdAsync(Guid id);
    Task<(IEnumerable<QuestionDTO> Questions, int TotalItemCount)> GetItemsAsync(
        PaginationDTO paginationDTO,
        QuestionSearchDTO searchDTO,
        bool onlyPublic = true);
    Task<QuestionDTO> AddAsync(QuestionForCreationDTO questionDTO);
    Task UpdateAsync(Guid id, QuestionForPutDTO questionDTO);
    Task DeleteAsync(Guid id);
}
