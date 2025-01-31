using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class QuestionRepository : IQuestionRepository
{
    private readonly QAPlatformContext _dbContext;

    public QuestionRepository(QAPlatformContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Question>> GetAllAsync(
        int? limit,
        string? searchString
    )
    {
        var query = _dbContext.Questions.AsQueryable();

        query = ApplySearchFilter(query, searchString);

        query = query.Include(q => q.Tags);

        if (limit.HasValue)
        {
            query = query.Take(limit.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Question>> GetAllPublicAsync(
        int? limit,
        string? searchString
    )
    {
        var query = _dbContext.Questions
            .Where(q => !q.IsProtected)
            .AsQueryable();

        query = ApplySearchFilter(query, searchString);

        query = query.Include(q => q.Tags);

        if (limit.HasValue)
        {
            query = query.Take(limit.Value);
        }

        return await query.ToListAsync();
    }


    private IQueryable<Question> ApplySearchFilter(
        IQueryable<Question> queryable,
        string? searchString
    )
    {
        if (string.IsNullOrWhiteSpace(searchString))
        {
            return queryable;
        }

        var query = $"%{searchString}%";

        return queryable.Where(q =>
            EF.Functions.ILike(q.Title, query) ||

            q.Tags.Any(tag => EF.Functions.ILike(tag.Value, query)) ||

            _dbContext.Topics
                .Where(t => t.Id == q.TopicId)
                .Any(t => EF.Functions.ILike(t.Name, query)) ||

            _dbContext.Subjects
                .Where(s => _dbContext.Topics
                    .Where(t => t.Id == q.TopicId && t.SubjectId == s.Id)
                    .Any())
                .Any(s => EF.Functions.ILike(s.Name, query) ||
                    (s.SubjectCode != null && EF.Functions.ILike(s.SubjectCode, query)))
        );
    }


    public async Task<Question?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Questions.FindAsync(id);
    }

    public async Task<Question> AddAsync(Question question)
    {
        _dbContext.Questions.Add(question);
        await _dbContext.SaveChangesAsync();
        return question;
    }

    public async Task UpdateAsync(Question question)
    {
        _dbContext.Questions.Update(question);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var question = await _dbContext.Questions.FindAsync(id);
        if (question != null)
        {
            _dbContext.Questions.Remove(question);
            await _dbContext.SaveChangesAsync();
        }
    }

}
