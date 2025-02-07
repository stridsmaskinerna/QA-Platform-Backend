using Domain.Entities;

namespace Application.Tests.Utilities;

public static class AnswerVoteFactory
{
    public static AnswerVotes CreateAnswerVoteEntity(
        Guid answerId,
        User user,
        Answer answer,
        bool vote = false
    ) => new()
    {
        AnswerId = answerId,
        UserId = user.Id,
        User = user,
        Answer = answer,
        Vote = vote
    };
}
