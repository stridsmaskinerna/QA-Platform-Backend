namespace Domain.DTO.Query;

public class QuestionSearchDTO
{
    public string? SearchString { get; set; }
    public string? SubjectCode { get; set; }
    public string? SubjectName { get; set; }
    public string? TopicName { get; set; }
    public bool? IsResolved { get; set; }
    public bool OnlyPublic { get; set; } = true;
}
