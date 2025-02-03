namespace Domain.Entities;

public class Tag
{
    public Guid Id { get; set; }

    public string Value { get; set; } = string.Empty;

    // Navigation

    public ICollection<Question> Questions { get; set; } = [];
}
