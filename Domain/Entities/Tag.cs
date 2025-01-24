namespace Domain.Entities;

public class Tag
{
    public Guid Id { get; set; }

    public string Value { get; set; } = null!;

    // Navigation

    public IEnumerable<Question> Questions { get; set; } = [];
}
