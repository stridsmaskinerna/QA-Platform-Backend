using Domain.DTO.Query;
using Domain.Entities;

namespace Domain.Contracts;

public interface ITagRepository
{
    Task<Tag?> GetByIdAsync(Guid id);
    Task<Tag?> GetByValueAsync(string value);
    Task<(IEnumerable<Tag> Tags, int TotalItemCount)> GetAllAsync(PaginationDTO pagination);
    Task<IEnumerable<Tag>> GetFilteredList(string value);
    Task<Tag> AddAsync(Tag tag);
    Task UpdateAsync(Tag tag);
    Task DeleteAsync(Guid id);
    Task<bool> IsTagValueTakenAsync(string tagValue);
    Task DeleteUnusedTagsAsync(IEnumerable<Tag> tags);
}
