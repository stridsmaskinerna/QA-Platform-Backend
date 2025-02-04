using System.Data;
using Domain.DTO.Query;
using Domain.Entities;
using Infrastructure.Contexts;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class QuestionRepository : IQuestionRepository
{
    private readonly QAPlatformContext _dbContext;

    public QuestionRepository(QAPlatformContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Question?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Questions
            .Where(q => q.Id == id)
            .Include(q => q.Topic)
            .ThenInclude(t => t.Subject)
            .Include(q => q.Tags)
            .Include(q => q.User)
            .Include(q => q.Answers)
            .FirstOrDefaultAsync();
    }

    public async Task<Question> AddAsync(Question question)
    {
        _dbContext.Questions.Add(question);
        await _dbContext.SaveChangesAsync();
        return await GetByIdAsync(question.Id) ?? throw new DataException();
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

    public async Task CompleteAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public async
    Task<(IEnumerable<Question> Questions, int TotalItemCount)>
    GetItemsAsync(
        PaginationDTO paginationDTO,
        QuestionSearchDTO searchDTO,
        bool onlyPublic
    )
    {
        var totalItemCount = await _dbContext.Questions.CountAsync();

        var query = _dbContext.Questions.AsQueryable();

        query = query
            .Include(q => q.Topic)
            .ThenInclude(t => t.Subject)
            .Include(q => q.Tags)
            .Include(q => q.Answers)
            .Include(q => q.User)
            .OrderBy(q => q.Created);

        query = query
            .Pipe(q => ApplyPublicFilter(q, onlyPublic))
            .Pipe(q => ApplyResolvedFilter(q, searchDTO))
            .Pipe(q => ApplySubjectAndTopiFilter(q, searchDTO))
            .Pipe(q => ApplySearchFilter(q, searchDTO))
            .Pipe(q => ApplySorting(q))
            .Pipe(q => ApplyPagination(q, paginationDTO));

        return (
            Questions: await query.ToListAsync(),
            TotalItemCount: totalItemCount
        );
    }


    private IQueryable<Question> ApplyPublicFilter(
        IQueryable<Question> queryable,
        bool onlyPublic
    )
    {
        if (onlyPublic)
        {
            queryable = queryable.Where(q => !q.IsProtected);
        }

        return queryable;
    }


    private IQueryable<Question> ApplyResolvedFilter(
        IQueryable<Question> queryable,
        QuestionSearchDTO searchDTO
    )
    {
        if (searchDTO.IsResolved == null)
        {
            return queryable;
        }

        return searchDTO.IsResolved == true
            ? queryable.Where(q => q.IsResolved)
            : queryable.Where(q => !q.IsResolved);
    }

    private IQueryable<Question> ApplySubjectAndTopiFilter(
        IQueryable<Question> queryable,
        QuestionSearchDTO searchDTO
    )
    {
        if (searchDTO.SubjectId is not null)
        {
            queryable = queryable.Where(q => q.Topic.Subject.Id == searchDTO.SubjectId);
        }

        if (searchDTO.TopicId is not null)
        {
            queryable = queryable.Where(q => q.Topic.Id == searchDTO.TopicId);
        }

        return queryable;
    }

    private IQueryable<Question> ApplySearchFilter(
        IQueryable<Question> queryable,
        QuestionSearchDTO searchDTO
    )
    {
        if (string.IsNullOrWhiteSpace(searchDTO.SearchString))
        {
            return queryable;
        }

        var query = $"%{searchDTO.SearchString}%";

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

    private IQueryable<Question> ApplySorting(
        IQueryable<Question> queryable
    )
    {
        return queryable.OrderBy(q => q.Created);
    }

    private IQueryable<Question> ApplyPagination(
        IQueryable<Question> queryable,
        PaginationDTO paginationDTO
    )
    {
        return queryable
            .Skip(paginationDTO.Limit * (paginationDTO.PageNr - 1))
            .Take(paginationDTO.Limit);
    }

}
