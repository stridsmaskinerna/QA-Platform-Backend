namespace Domain.DTO.Response;

public class CommentDTO
{
    public Guid Id { get; set; }
    public string? UserName { get; set; }
    public required string Value { get; set; }
}
