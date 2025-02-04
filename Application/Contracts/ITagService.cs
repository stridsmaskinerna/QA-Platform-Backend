using Domain.Entities;

namespace Application.Contracts;

public interface ITagService
{
    Task<Tag?> GetByIdAsync(Guid id);
    Task<Tag?> GetByValueAsync(string value);
    Task<IEnumerable<Tag>> GetAllAsync();
    Task<Tag> AddAsync(Tag tag);
    Task UpdateAsync(Tag tag);
    Task DeleteAsync(Guid id);
    Task<bool> IsTagValueTakenAsync(string tagValue);
}
