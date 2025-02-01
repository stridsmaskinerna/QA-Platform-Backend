using System.IO.Compression;
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
        string? searchString,
        Guid? subjecId,
        Guid? topicId,
        string? resolvedFilter,
        bool onlyPublic = true
    )
    {

        var query = _dbContext.Questions.AsQueryable();

        if (onlyPublic)
        {
            query = query.Where(q => !q.IsProtected);
        }

        query = query.Include(q => q.Topic).ThenInclude(t => t.Subject);
        query = ApplySearchFilter(query, searchString, subjecId, topicId, resolvedFilter);

        query = query.Include(q => q.Tags);
        query = query.Include(q => q.Answers);
        query = query.Include(q => q.User);
        query = query.OrderBy(q => q.Created);

        if (limit.HasValue)
        {
            query = query.Take(limit.Value);
        }

        return await query.OrderBy(q => q.Created).ToListAsync();
    }


    private IQueryable<Question> ApplySearchFilter(
        IQueryable<Question> queryable,
        string? searchString,
        Guid? subjectId,
        Guid? topicId,
        string? resolvedFilter
    )
    {
        if (resolvedFilter is not null)
        {
            if (resolvedFilter == "onlyResolved")
            {
                queryable = queryable.Where(q => q.IsResolved);
            }
            else if (resolvedFilter == "onlyUnresolved")
            {
                queryable = queryable.Where(q => !q.IsResolved);
            }
        }

        if (subjectId is not null)
        {
            queryable = queryable.Where(q => q.Topic.Subject.Id == subjectId);

            if (topicId is not null)
            {
                queryable = queryable.Where(q => q.Topic.Id == topicId);
            }
        }

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
