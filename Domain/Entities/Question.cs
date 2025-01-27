namespace Domain.Entities;

public class Question
{
    public Guid Id { get; set; }

    public Guid TopicId { get; set; }

    public string? UserId { get; set; }

    public string Title { get; set; } = String.Empty;

    public string Description { get; set; } = String.Empty;

    public string? FilePath { get; set; }

    public DateTime Created { get; set; }

    public bool IsResolved { get; set; }

    public bool IsProtected { get; set; }

    public bool IsHidden { get; set; }

    // Navigation

    public ICollection<Answer> Answers { get; set; } = [];

    public ICollection<Tag> Tags { get; set; } = [];
}
