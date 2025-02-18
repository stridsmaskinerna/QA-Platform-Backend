namespace Domain.DTO.Request;

public class TopicForCreationDTO
{
    public string Name { get; set; } = string.Empty;
    public Guid SubjectId { get; set; }
    public bool IsActive { get; set; } = true;
}
