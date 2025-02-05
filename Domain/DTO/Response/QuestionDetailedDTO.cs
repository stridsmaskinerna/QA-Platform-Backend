namespace Domain.DTO.Response;

public class QuestionDetailedDTO : QuestionDTO
{
    public ICollection<AnswerDetailedDTO>? Answers { get; set; }
}
