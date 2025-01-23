namespace Infrastructure.Entities;

public class Subject
{
    public Guid Id { get; set; }
    public string Name {  get; set; } = String.Empty;
    public string? SubjectCode { get; set; }

    // Navigation
    public IEnumerable<Topic> Topics { get; set; } = [];

}
