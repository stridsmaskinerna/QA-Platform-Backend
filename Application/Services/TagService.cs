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

    public static string MsgNotFound() => "Tag not in the database";

    public async Task<TagStandardDTO> AddAsync(Tag tag)
    {
        return _sm.Mapper.Map<TagStandardDTO>(
            await _rm.TagRepository.AddAsync(tag));
    }

    public async Task DeleteAsync(Guid id)
    {
        var tag = await _rm.TagRepository.GetByIdAsync(id);
        if (tag == null)
        {
            NotFound(MsgNotFound());
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
        var tag = await _rm.TagRepository.GetByIdAsync(id);
        if (tag == null)
        {
            NotFound(MsgNotFound());
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

    public async Task StoreNewTagsFromQuestion(
        Question question,
        IEnumerable<string> tags
    )
    {
        var normalizedNewTagValues = tags
            .Select(_sm.UtilityService.NormalizeText)
            .ToList();

        foreach (var tagValue in normalizedNewTagValues)
        {
            var tag = await _rm.TagRepository.GetByValueAsync(tagValue);

            if (tag == null)
            {
                tag = new Tag { Value = tagValue };
                await _rm.TagRepository.AddAsync(tag);
            }

            if (!question.Tags.Any(t => t.Value == tagValue))
            {
                question.Tags.Add(tag);
            }
        }
    }
}
