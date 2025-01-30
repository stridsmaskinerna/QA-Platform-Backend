namespace Domain.DTO.Request;

public class CommentForCreationDTO
{
    public Guid AnswerId { get; set; }
    public required string Value { get; set; }

}
