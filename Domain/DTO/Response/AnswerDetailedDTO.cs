using Domain.Constants;

namespace Domain.DTO.Response;

public class AnswerDetailedDTO : AnswerDTO
{
    public string MyVote { get; set; } = VoteType.NEUTRAL;
    public bool AnsweredByTeacher { get; set; }
    public bool IsHideable { get; set; } = false;
}
