using Domain.DTO.Query;
using Domain.DTO.Request;
using Domain.DTO.Response;

namespace Application.Contracts;

public interface IQuestionService
{
    Task<QuestionDetailedDTO> GetByIdAsync(Guid id);
    Task<QuestionForEditDTO> GetByIdForEditAsync(Guid id);
    Task<(IEnumerable<QuestionDTO> Questions, int TotalItemCount)> GetItemsAsync(
        PaginationDTO paginationDTO,
        QuestionSearchDTO searchDTO);
    Task<(IEnumerable<QuestionDTO> Questions, int TotalItemCount)> GetPublicItemsAsync(
        PaginationDTO paginationDTO,
        QuestionSearchDTO searchDTO);
    Task<QuestionDTO> AddAsync(QuestionForCreationDTO questionDTO);
    Task UpdateAsync(Guid id, QuestionForPutDTO questionDTO);
    Task DeleteAsync(Guid id);
    Task<QuestionDetailedDTO> GetPublicQuestionByIdAsync(Guid id);
    Task<(IEnumerable<QuestionDTO> Questions, int TotalItemCount)> GetTeacherQuestionsAsync(PaginationDTO paginationDTO, Guid subjectId);
    Task ManageQuestionVisibilityAsync(Guid id);
}
