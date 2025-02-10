using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;

namespace Application.Tests.Utilities;

public class QuestionFactory
{
    public static QuestionDetailedDTO CreateQuestionDetailedDto(
        Guid id,
        Guid topicId
    ) => new()
    {
        Id = id,
        TopicName = "Test TopicName",
        TopicId = topicId,
        SubjectName = "Test TopicName",
        UserName = "Test UserName",
        Title = "Test Title",
        Description = "Test Description"
    };

    public static QuestionDTO CreateQuestionDto(
        Guid id,
        Guid topicId
    ) => new()
    {
        Id = id,
        TopicName = "Test TopicName",
        TopicId = topicId,
        SubjectName = "Test TopicName",
        UserName = "Test UserName",
        Title = "Test Title",
        Description = "Test Description"
    };

    public static QuestionForPutDTO CreateQuestionForPutDto(
        Guid id,
        Guid topicId
    ) => new()
    {
        Title = "Test Title",
        Description = "Test Description"
    };

    public static QuestionForCreationDTO CreateQuestionForCreationDto(
        Guid id,
        Guid topicId
    ) => new()
    {
        Title = "Test Title",
        Description = "Test Description"
    };

    public static Question CreateQuestionEntity(
        Guid id,
        Topic topic,
        User user
    ) => new()
    {
        Id = id,
        Topic = topic,
        TopicId = topic.Id,
        User = user,
        UserId = user.Id,
        Title = "Test Title",
        Description = "Test Description"
    };
}
