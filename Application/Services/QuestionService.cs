using Domain.Entities;
using Infrastructure.Repositories;

namespace Application.Services;

public class QuestionService : BaseService, IQuestionService
{
    private readonly IQuestionRepository _repository;

    public QuestionService(IQuestionRepository rp)
    {
        _repository = rp;
    }

    public async Task<IEnumerable<Question>> GetAllAsync(
        int? limit,
        string? searchString,
        Guid? subjectId,
        Guid? topicId,
        string? resolvedFilter,
        bool onlyPublic = true
    )
    {
        return await _repository.GetAllAsync(limit, searchString, subjectId, topicId, resolvedFilter, onlyPublic);
    }

    public async Task<Question?> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Question> AddAsync(Question question)
    {
        return await _repository.AddAsync(question);
    }

    public async Task UpdateAsync(Question question)
    {
        await _repository.UpdateAsync(question);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }
}

