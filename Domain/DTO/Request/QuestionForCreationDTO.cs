namespace Domain.DTO.Request;

public class QuestionForCreationDTO
{
    public Guid SubjectId { get; set; }
    public Guid TopicId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public bool IsProtected { get; set; }
    public ICollection<string>? Tags { get; set; }
}
