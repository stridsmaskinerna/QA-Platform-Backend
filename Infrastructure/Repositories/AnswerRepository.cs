using Domain.Contracts;
using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AnswerRepository : IAnswerRepository
{
    private readonly QAPlatformContext _dbContext;

    public AnswerRepository(QAPlatformContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Answer?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Answers.FindAsync(id);
    }

    public async Task<Answer> AddAsync(Answer answer)
    {
        _dbContext.Answers.Add(answer);
        await _dbContext.SaveChangesAsync();
        return answer;
    }

    public async Task UpdateAsync(Answer answer)
    {
        _dbContext.Answers.Update(answer);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Answer answer)
    {
        _dbContext.Answers.Remove(answer);
        await _dbContext.SaveChangesAsync();
    }

    public async Task CompleteAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Answer>> GetAnswersByQuestionIdAsync(Guid questionId)
    {
        return await _dbContext.Answers.Where(a => a.QuestionId == questionId).ToListAsync();
    }

    public async Task<IEnumerable<Answer>> GetAnswersByUserIdAsync(Guid userId)
    {
        return await _dbContext.Answers.Where(a => a.UserId == userId.ToString()).ToListAsync();
    }

    public async Task<int> GetAnswerCountByQuestionIdAsync(Guid questionId)
    {
        return await _dbContext.Answers.CountAsync(a => a.QuestionId == questionId);
    }
}
