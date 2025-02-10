using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;

namespace Application.Tests.Utilities;

public static class AnswerFactory
{
    public static Answer CreateAnswerEntity(
        Guid id,
        Question question,
        User user
    ) => new()
    {
        Id = id,
        Value = "Test Answer",
        Question = question,
        User = user
    };

    public static AnswerForCreationDTO CreateAnswerForCreationDTO() => new()
    {
        Value = "Test Answer"
    };

    public static AnswerForPutDTO CreateAnswerForPutDTO() => new()
    {
        Value = "Test Answer"
    };

    public static AnswerDTO CreateAnswerDTO(
        Answer answer,
        string userName
    ) => new()
    {
        Id = answer.Id,
        Value = answer.Value,
        UserName = userName
    };

    public static AnswerDetailedDTO CreateAnswerDetailedDTO(
        Answer answer,
        string userName
    ) => new()
    {
        Id = answer.Id,
        Value = answer.Value,
        UserName = userName
    };
}
