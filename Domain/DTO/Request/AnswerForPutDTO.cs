namespace Domain.DTO.Request;

public class AnswerForPutDTO
{
    public required string Value { get; set; }
    public string? FilePath { get; set; }
}
