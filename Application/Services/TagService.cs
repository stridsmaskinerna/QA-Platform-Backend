using Application.Contracts;
using Domain.Contracts;
using Domain.DTO.Response;
using Domain.Entities;

namespace Application.Services;

public class TagService : BaseService, ITagService
{

    public readonly IRepositoryManager _rm;
    private readonly IServiceManager _sm;

    public TagService(
        IRepositoryManager rm,
        IServiceManager sm
    )
    {
        _rm = rm;
        _sm = sm;
    }

    public async Task<TagStandardDTO> AddAsync(Tag tag)
    {
        return _sm.Mapper.Map<TagStandardDTO>(
            await _rm.TagRepository.AddAsync(tag));
    }

    public async Task DeleteAsync(Guid id)
    {
        var tagToRemove = await _sm.TagService.GetByIdAsync(id);
        if (tagToRemove == null)
        {
            NotFound("Tag not in the database");
        }
        await _rm.TagRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<TagStandardDTO>> GetAllAsync()
    {
        return _sm.Mapper.Map<IEnumerable<TagStandardDTO>>(
            await _rm.TagRepository.GetAllAsync());
    }

    public async Task<IEnumerable<TagStandardDTO>> GetFilteredList(string value)
    {
        return _sm.Mapper.Map<IEnumerable<TagStandardDTO>>(
            await _rm.TagRepository.GetFilteredList(value));
    }

    public async Task<TagStandardDTO> GetByIdAsync(Guid id)
    {
        var tag = await _sm.TagService.GetByIdAsync(id);
        if (tag == null)
        {
            NotFound("Tag not in the database");
        }
        return _sm.Mapper.Map<TagStandardDTO>(tag);
    }

    public async Task<TagStandardDTO> GetByValueAsync(string value)
    {
        return _sm.Mapper.Map<TagStandardDTO>(
            await _rm.TagRepository.GetByValueAsync(value));
    }

    public async Task<bool> IsTagValueTakenAsync(string tagValue)
    {
        return await _rm.TagRepository.IsTagValueTakenAsync($"{tagValue}");
    }

    public async Task UpdateAsync(Tag tag)
    {
        await _rm.TagRepository.UpdateAsync(tag);
    }
}
