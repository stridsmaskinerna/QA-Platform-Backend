namespace Domain.Entities;

public class Comment
{
    public Guid Id { get; set; }

    public Guid AnswerId { get; set; }

    public string? UserId { get; set; }

    public string Value { get; set; } = String.Empty;

    public required User User { get; set; }

    public required Answer Answer { get; set; }
}
