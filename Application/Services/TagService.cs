using Application.Contracts;
using Domain.Entities;
using Infrastructure.Repositories;

namespace Application.Services;

public class TagService : ITagService
{

    public readonly ITagRepository _repository;

    public TagService(ITagRepository repository) => _repository = repository;

    public async Task<Tag> AddAsync(Tag tag)
    {
        return await _repository.AddAsync(tag);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<Tag>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<IEnumerable<Tag>> GetFilteredList(string value)
    {
        return await _repository.GetFilteredList(value);
    }

    public async Task<Tag?> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Tag?> GetByValueAsync(string value)
    {
        return await _repository.GetByValueAsync(value);
    }

    public async Task<bool> IsTagValueTakenAsync(string tagValue)
    {
        return await _repository.IsTagValueTakenAsync($"{tagValue}");
    }

    public async Task UpdateAsync(Tag tag)
    {
        await _repository.UpdateAsync(tag);
    }
}
