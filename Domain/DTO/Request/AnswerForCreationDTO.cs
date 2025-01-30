namespace Domain.DTO.Request;

public class AnswerForCreationDTO
{
    public Guid QuestionId { get; set; }
    public required string Value { get; set; }
    public string? FilePath { get; set; }
}
