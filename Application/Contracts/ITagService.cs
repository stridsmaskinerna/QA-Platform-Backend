using Domain.DTO.Query;
using Domain.DTO.Response;
using Domain.Entities;

namespace Application.Contracts;

public interface ITagService
{
    Task<TagStandardDTO> GetByIdAsync(Guid id);
    Task<TagStandardDTO> GetByValueAsync(string value);
    Task<(IEnumerable<TagStandardDTO> Tags, int TotalItemCount)> GetAllAsync(
        PaginationDTO paginationDTO);
    Task<IEnumerable<TagStandardDTO>> GetFilteredList(string value);
    Task<TagStandardDTO> AddAsync(Tag tag);
    Task UpdateAsync(Tag tag);
    Task DeleteAsync(Guid id);
    Task<bool> IsTagValueTakenAsync(string tagValue);
    Task StoreNewTagsFromQuestion(Question question, IEnumerable<string> tags);
}
