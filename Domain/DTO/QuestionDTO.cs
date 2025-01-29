namespace Domain.DTO;

public class QuestionDTO
{
    public Guid Id { get; set; }
    public required string TopicName { get; set; }
    public required string SubjectName { get; set; }
    public string? SubjectCode { get; set; }
    public required string UserName { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public string? FilePath { get; set; }
    public DateTime Created { get; set; }
    public bool IsResolved { get; set; }
    public bool IsProtected { get; set; }
    public bool IsHidden { get; set; }
    public int AnswerCount { get; set; }
    public ICollection<string>? Tags { get; set; }
}
