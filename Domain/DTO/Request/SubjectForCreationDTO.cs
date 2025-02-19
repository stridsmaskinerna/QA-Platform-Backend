namespace Domain.DTO.Request;

public class SubjectForCreationDTO
{
    public string Name { get; set; } = string.Empty;
    public string? SubjectCode { get; set; }
    public ICollection<string> Teachers { get; set; } = [];
}
