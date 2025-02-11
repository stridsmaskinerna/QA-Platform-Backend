namespace Domain.DTO.Response;

public class SubjectDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? SubjectCode { get; set; }
    public IEnumerable<UserWithEmailDTO> Teachers { get; set; } = [];
    public IEnumerable<TopicDTO> Topics { get; set; } = [];
}
