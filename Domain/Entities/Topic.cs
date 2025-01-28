namespace Domain.Entities;

public class Topic
{
    public Guid Id { get; set; }

    public Guid SubjectId { get; set; }

    public string Name { get; set; } = String.Empty;

    public bool IsActive { get; set; }

    // Navigation
    public IEnumerable<Question> Questions { get; set; } = [];

}
