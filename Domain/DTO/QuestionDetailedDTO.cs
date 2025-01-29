namespace Domain.DTO;

public class QuestionDetailedDTO : QuestionDTO
{
    public ICollection<AnswerDTO>? Answers { get; set; }

}
