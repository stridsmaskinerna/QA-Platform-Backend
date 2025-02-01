using Domain.Entities;

public class AnswerVotes
{
    public required string UserId { get; set; }
    public Guid AnswerId { get; set; }

    public bool Vote { get; set; }

    public required User User { get; set; }

    public required Answer Answer { get; set; }
}
