namespace Domain.DTO;

public class AnswerDTO
{
    public Guid Id { get; set; }
    public required string UserName { get; set; }
    public required string Value { get; set; }
    public int Rating { get; set; }
    public string? FilePath { get; set; }
    public DateTime Created { get; set; }
    public bool IsHidden { get; set; }
    public bool IsTeacher { get; set; }
    public bool IsAccepted { get; set; }
    public IEnumerable<CommentDTO>? Comments { get; set; }
}
