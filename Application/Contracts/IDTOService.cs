using Domain.DTO.Response;
using Domain.Entities;

namespace Application.Contracts;

public interface IDTOService
{
    void UpdateAnswerIsHideableField(QuestionDetailedDTO questionDTO);
    Task UpdateQuestionIsHideableField(IEnumerable<QuestionDTO> DTOList, string userId);
    Task UpdateQuestionIsHideableField(QuestionDTO dto, string userId);
    Task UpdatingAnsweredByTeacherField(QuestionDetailedDTO questionDTO);
    void UpdatingMyVoteField(Question question, QuestionDetailedDTO questionDTO, string? userId);
}
