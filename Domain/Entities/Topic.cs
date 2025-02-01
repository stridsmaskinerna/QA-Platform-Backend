namespace Domain.Entities;

public class Topic
{
    public Guid Id { get; set; }

    public Guid SubjectId { get; set; }

    public string Name { get; set; } = String.Empty;

    public bool IsActive { get; set; }

    // Navigation
    public ICollection<Question> Questions { get; set; } = [];

    public required Subject Subject { get; set; }

}
