namespace Infrastructure.Entities;

public class Answer
{
    public Guid Id { get; set; }

    public string Value { get; set; } = String.Empty;

    public int Rating { get; set; }

    public string FilePath { get; set; } = String.Empty;

    public DateTime Created { get; set; }

    public bool IsHidden { get; set; }

    // Navigation

    public IEnumerable<Comment> Comments { get; set; } = [];
}
