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

    public async Task<IEnumerable<Question>> GetAllAsync(
        PaginationDTO paginationDTO,
        QuestionSearchDTO searchDTO
    )
    {
        var query = _dbContext.Questions.AsQueryable();

        query = query.Include(q => q.Topic)
            .ThenInclude(t => t.Subject)
            .Include(q => q.Tags)
            .Include(q => q.Answers)
            .Include(q => q.User)
            .OrderBy(q => q.Created);

        query = query
            .Pipe(q => ApplyPublicFilter(q, searchDTO))
            .Pipe(q => ApplyResolvedFilter(q, searchDTO))
            .Pipe(q => ApplySubjectAndTopiFilter(q, searchDTO))
            .Pipe(q => ApplySearchFilter(q, searchDTO))
            .Pipe(q => ApplySorting(q))
            .Pipe(q => ApplyPagination(q, paginationDTO));

        return await query.ToListAsync();
    }


    private IQueryable<Question> ApplyPublicFilter(
        IQueryable<Question> queryable,
        QuestionSearchDTO searchDTO
    )
    {
        if (searchDTO.OnlyPublic)
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
        if (searchDTO.SubjectName is not null)
        {
            queryable = queryable.Where(q => q.Topic.Subject.Name == searchDTO.SubjectName);
        }

        if (searchDTO.SubjectCode is not null)
        {
            queryable = queryable.Where(q => q.Topic.Subject.SubjectCode == searchDTO.SubjectCode);
        }

        if (searchDTO.TopicName is not null)
        {
            queryable = queryable.Where(q => q.Topic.Name == searchDTO.TopicName);
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
