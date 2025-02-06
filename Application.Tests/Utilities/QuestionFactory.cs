using Domain.DTO.Response;
using Domain.Entities;

namespace Application.Tests.Utilities;

public class QuestionFactory
{
    public static QuestionDetailedDTO CreateQuestionDetailedDto(
        Guid topicId
    ) => new()
    {
        TopicName = "Test TopicName",
        TopicId = topicId,
        SubjectName = "Test TopicName",
        UserName = "Test UserName",
        Title = "Test Title",
        Description = "Test Description"
    };

    public static QuestionDTO CreateQuestionDto(
        Guid topicId
    ) => new()
    {
        TopicName = "Test TopicName",
        TopicId = topicId,
        SubjectName = "Test TopicName",
        UserName = "Test UserName",
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
