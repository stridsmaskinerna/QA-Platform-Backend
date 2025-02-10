using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;

namespace Application.Tests.Utilities;

public static class CommentFactory
{
    public static Comment CreateCommentEntity(
        Guid id,
        Answer answer,
        User user
    ) => new()
    {
        Id = id,
        AnswerId = answer.Id,
        Value = "Test Comment",
        Answer = answer,
        User = user
    };

    public static CommentForCreationDTO CreateCommentForCreationDTO(
        Guid answerId
    ) => new()
    {
        AnswerId = answerId,
        Value = "Test Comment"
    };

    public static CommentForPutDTO CreateCommentForPutDTO() => new()
    {
        Value = "Test Comment"
    };

    public static CommentDTO CreateCommentDTO(
        Comment comment,
        string userName
    ) => new()
    {
        Id = comment.Id,
        UserName = userName,
        Value = comment.Value
    };
}
