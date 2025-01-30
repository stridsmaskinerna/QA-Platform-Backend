namespace Domain.DTO.Response;

public class QuestionDetailedDTO : QuestionDTO
{
    public ICollection<AnswerDTO>? Answers { get; set; }

}
