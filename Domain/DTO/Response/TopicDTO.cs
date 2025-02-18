namespace Domain.DTO.Response;

public class TopicDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public Guid SubjectId { get; set; }

}
