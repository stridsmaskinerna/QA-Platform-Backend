using Domain.Contracts;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AnswerVoteRepository : IAnswerVoteRepository
{
    private readonly QAPlatformContext _dbContext;

    public AnswerVoteRepository(QAPlatformContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AnswerVotes?> GetAsync(Guid answerId, string userId)
    {
        return await _dbContext.AnswerVotes
            .Include(av => av.Answer)
            .FirstOrDefaultAsync(av =>
                av.AnswerId == answerId &&
                av.UserId == userId);
    }

    public async Task<AnswerVotes> AddAsync(AnswerVotes answerVotes)
    {
        _dbContext.AnswerVotes.Add(answerVotes);
        await _dbContext.SaveChangesAsync();
        return answerVotes;
    }

    public async Task UpdateAsync(AnswerVotes answerVotes)
    {
        _dbContext.AnswerVotes.Update(answerVotes);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid answerId, string userId)
    {
        var answerVotes = await GetAsync(answerId, userId);

        if (answerVotes != null)
        {
            _dbContext.AnswerVotes.Remove(answerVotes);
            await _dbContext.SaveChangesAsync();
        }
    }
}
