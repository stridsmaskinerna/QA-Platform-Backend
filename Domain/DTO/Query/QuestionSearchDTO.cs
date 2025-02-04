namespace Domain.DTO.Query;

public class QuestionSearchDTO
{
    public string? SearchString { get; set; }
    public Guid? SubjectId { get; set; }
    public Guid? TopicId { get; set; }
    public bool? IsResolved { get; set; }
    public string? InteractionType { get; set; }
}
