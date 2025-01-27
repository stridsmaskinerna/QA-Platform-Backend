namespace Domain.Entities;

public class Subject
{
    public Guid Id { get; set; }

    public string Name {  get; set; } = String.Empty;

    public string? SubjectCode { get; set; }

    // Navigation
    public ICollection<Topic> Topics { get; set; } = [];

    public ICollection<User> Teachers { get; set; } = [];
}
