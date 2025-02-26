namespace Domain.Entities;

public class Answer
{
    public Guid Id { get; set; }

    public Guid QuestionId { get; set; }

    public string? UserId { get; set; }

    public string Value { get; set; } = string.Empty;

    public string? FilePath { get; set; }

    public DateTime Created { get; set; }

    public bool IsHidden { get; set; }

    public bool IsAccepted { get; set; }

    // Navigation

    public ICollection<Comment> Comments { get; set; } = [];

    public required Question Question { get; set; }

    public required User User { get; set; }

    public ICollection<AnswerVotes> AnswerVotes { get; set; } = [];
}
