namespace Domain.Contracts;

public interface IAnswerVoteRepository
{
    Task<AnswerVotes?> GetAsync(Guid answerId, string userId);
    Task<AnswerVotes> AddAsync(AnswerVotes answerVotes);
    Task DeleteAsync(Guid answerId, string userId);
    Task UpdateAsync(AnswerVotes answerVotes);
}
