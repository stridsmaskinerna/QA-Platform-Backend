using System.Data;
using System.IO.Compression;
using Domain.Constants;
using Domain.Contracts;
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

    public async Task<Question?> GetByIdAsync(Guid id, string? userId = null, List<string>? userRoles = null)
    {
        var question = await _dbContext.Questions
            .Where(q => q.Id == id)
            .Include(q => q.Topic)
                .ThenInclude(t => t.Subject)
                .ThenInclude(s => s.Teachers)
            .Include(q => q.Tags)
            // Include Users under questions used for DTO mapping
            .Include(q => q.User)
            .Include(q => q.Answers)
                .ThenInclude(a => a.Comments)
                // Include Users under comments used for DTO mapping
                .ThenInclude(c => c.User)
            .Include(q => q.Answers)
                // Include Users under answers used for DTO mapping
                .ThenInclude(a => a.User)
            .Include(q => q.Answers)
                // Include AnswerVotes under answers used for DTO mapping
                .ThenInclude(a => a.AnswerVotes).FirstOrDefaultAsync();

        ApplyAnswerIsHiddenFilter(question, userRoles, userId);

        return question;
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

    public async Task<(IEnumerable<Question> Questions, int TotalItemCount)> GetTeacherQuestionsAsync(
        PaginationDTO paginationDTO,
        Guid subjectId,
        User teacher
    )
    {
        var query = _dbContext.Questions.AsQueryable();

        query = query
            .Include(q => q.Topic)
            .ThenInclude(t => t.Subject)
            .ThenInclude(s => s.Teachers)
            .Include(q => q.Tags)
            .Include(q => q.Answers)
            .Include(q => q.User);

        query = query
            .Pipe(q => ApplyUserFilter(q, teacher))
            .Pipe(q => ApplySubjectFilter(q, subjectId));

        var totalItemCount = await query.CountAsync();

        query = query
            .Pipe(ApplySorting)
            .Pipe(q => ApplyPagination(q, paginationDTO));



        return (
            Questions: await query.ToListAsync(),
            TotalItemCount: totalItemCount
        );
    }

    public async Task<(IEnumerable<Question> Questions, int TotalItemCount)> GetItemsAsync(
        PaginationDTO paginationDTO,
        QuestionSearchDTO searchDTO,
        bool onlyPublic,
        string? userId,
        List<string>? userRoles
    )
    {


        var query = _dbContext.Questions.AsQueryable();

        query = query
            .Include(q => q.Topic)
            .ThenInclude(t => t.Subject)
            .Include(q => q.Tags)
            .Include(q => q.Answers)
            .Include(q => q.User);

        query = query
            .Pipe(q => ApplyPublicFilter(q, onlyPublic))
            .Pipe(q => ApplyIsHiddenFilter(q, userRoles, userId))
            .Pipe(q => ApplyUserInteractionTypeFilter(q, searchDTO, userId))
            .Pipe(q => ApplyResolvedFilter(q, searchDTO))
            .Pipe(q => ApplySubjectFilter(q, searchDTO.SubjectId))
            .Pipe(q => ApplyTopicFilter(q, searchDTO.TopicId))
            .Pipe(q => ApplySearchFilter(q, searchDTO));

        var totalItemCount = await query.CountAsync();

        query = query
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
            return queryable.Where(q => !q.IsProtected);
        }
        return queryable;
    }

    private IQueryable<Question> ApplyIsHiddenFilter(IQueryable<Question> q, List<string>? userRoles, string? userId)
    {
        if (userId != null && userRoles != null && userRoles.Contains(DomainRoles.TEACHER))
        {
            return q.Where(q => !q.IsHidden || q.Topic.Subject.Teachers.Select(t => t.Id).Contains(userId));
        }
        return q.Where(q => !q.IsHidden);
    }

    private void ApplyAnswerIsHiddenFilter(Question? q, List<string>? userRoles, string? userId)
    {
        if (q == null ||
            (
            userId != null &&
            userRoles != null &&
            userRoles.Contains(DomainRoles.TEACHER) &&
            //If the user is teacher in the question's course, then dont filter out anything
            q.Topic.Subject.Teachers.Select(t => t.Id).Contains(userId)
            )

        )
        {
            return;
        }
        //If not a teacher in question's course then filter out all hidden answers
        foreach (var answer in q.Answers)
        {
            if (answer.IsHidden)
            {
                q.Answers.Remove(answer);
            }
        }

    }

    private IQueryable<Question> ApplyUserFilter(
        IQueryable<Question> queryable,
        User teacher
    )
    {
        return queryable.Where(q => q.Topic.Subject.Teachers.Contains(teacher));
    }

    private IQueryable<Question> ApplyUserInteractionTypeFilter(
         IQueryable<Question> queryable,
         QuestionSearchDTO searchDTO,
         string? userId
     )
    {
        if (userId is not null)
        {
            return searchDTO.InteractionType switch
            {
                InteractionType.CREATED => queryable.Where(
                    q => q.User.Id == userId),
                InteractionType.ANSWERED => queryable.Where(
                    q => q.Answers.Any(a => a.UserId == userId)),
                InteractionType.COMMENTED => queryable.Where(
                    q => q.Answers.Any(a => a.Comments.Any(c => c.UserId == userId))),
                _ => queryable
            };
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

    private IQueryable<Question> ApplySubjectFilter(
        IQueryable<Question> queryable,
        Guid? subjectId
    )
    {
        if (subjectId is not null)
        {
            queryable = queryable.Where(q => q.Topic.Subject.Id == subjectId);
        }

        return queryable;
    }

    private IQueryable<Question> ApplyTopicFilter(
        IQueryable<Question> queryable,
        Guid? topicId
    )
    {
        if (topicId is not null)
        {
            queryable = queryable.Where(q => q.Topic.Id == topicId);
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

        var searchStrings = searchDTO.SearchString.Split(
            //Build error (on Mac) if not explicitly typing the space, i.e new char[] { ' ' } or new string[] { " " },
            new char[] { ' ' },
            StringSplitOptions.RemoveEmptyEntries);

        foreach (var word in searchStrings)
        {
            var query = $"%{word}%";

            queryable = queryable.Where(q =>
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

        return queryable;
    }

    private IQueryable<Question> ApplySorting(
        IQueryable<Question> queryable
    )
    {
        return queryable.OrderByDescending(q => q.Created);
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
