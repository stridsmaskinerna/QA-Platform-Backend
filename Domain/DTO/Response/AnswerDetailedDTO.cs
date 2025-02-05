using Domain.Constants;

namespace Domain.DTO.Response;

public class AnswerDetailedDTO
{
    public Guid Id { get; set; }
    public string? UserName { get; set; }
    public required string Value { get; set; }
    public int Rating { get; set; }
    public string? FilePath { get; set; }
    public DateTime Created { get; set; }
    public bool IsHidden { get; set; }
    public int VoteCount { get; set; }
    public string MyVote { get; set; } = VoteType.NEUTRAL;
    public bool AnsweredByTeacher { get; set; }
    public bool IsAccepted { get; set; }
    public IEnumerable<CommentDTO> Comments { get; set; } = [];
}
