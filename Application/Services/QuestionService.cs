using Application.Contracts;
using Domain.Contracts;
using Domain.DTO.Query;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;
using Infrastructure.Repositories;

namespace Application.Services;

public class QuestionService : BaseService, IQuestionService
{
    private readonly IQuestionRepository _questionRepository;
    private readonly ITagRepository _tagRepository;
    private readonly ITopicRepository _topicRepostitory;
    private readonly IServiceManager _sm;

    public QuestionService(
        IQuestionRepository questionRepository,
        ITagRepository tagRepository,
        ITopicRepository topicRepository,
        IServiceManager sm
    )
    {
        _questionRepository = questionRepository;
        _tagRepository = tagRepository;
        _topicRepostitory = topicRepository;
        _sm = sm;
    }

    public async Task<(IEnumerable<Question> Questions, int TotalItemCount)> GetItemsAsync(
        PaginationDTO paginationDTO,
        QuestionSearchDTO searchDTO,
        bool onlyPublic = true
    )
    {
        var userId = _sm.TokenService.GetUserId();
        return await _questionRepository.GetItemsAsync(
            paginationDTO, searchDTO, userId, onlyPublic);
    }

    public async Task<QuestionDetailedDTO> GetByIdAsync(Guid id)
    {
        var question = await _questionRepository.GetByIdAsync(id);

        if (question == null)
        {
            NotFound();
        }

        return _sm.Mapper.Map<QuestionDetailedDTO>(question);
    }

    public async Task DeleteAsync(Guid id)
    {
        var question = await _questionRepository.GetByIdAsync(id);

        if (question == null)
        {
            NotFound();
        }

        await _questionRepository.DeleteAsync(id);
    }

    public async Task<QuestionDTO> AddAsync(QuestionForCreationDTO questionDTO)
    {
        var question = _sm.Mapper.Map<Question>(questionDTO);

        var topic = await _topicRepostitory.GetByIdAsync(questionDTO.TopicId);

        if (question == null || topic == null)
        {
            BadRequest("Invalid Question body");
        }

        question.UserId = _sm.TokenService.GetUserId();

        await AddNewTags(question, questionDTO.Tags);

        var createdQuestion = await _questionRepository.AddAsync(question);

        var createdQuestionDTO = _sm.Mapper.Map<QuestionDTO>(createdQuestion);

        createdQuestionDTO.UserName = _sm.TokenService.GetUserName();

        return createdQuestionDTO;
    }

    public async Task UpdateAsync(Guid id, QuestionForPutDTO questionDTO)
    {
        var question = await _questionRepository.GetByIdAsync(id);

        if (question == null)
        {
            NotFound($"No answer with id {id} exist.");
        }

        var normalizedNewTagValues = questionDTO.Tags
            .Select(_sm.UtilityService.NormalizeText)
            .ToList();

        var tagsToRemove = question.Tags
            .Where(t => !normalizedNewTagValues.Contains(t.Value))
            .ToList();

        foreach (var tag in tagsToRemove)
        {
            question.Tags.Remove(tag);
        }

        await _questionRepository.CompleteAsync();

        foreach (var tag in tagsToRemove)
        {
            await _tagRepository.DeleteUnusedTagsAsync(tag);
        }

        await AddNewTags(question, questionDTO.Tags);
    }

    private async Task AddNewTags(
        Question question,
        IEnumerable<string> tags
    )
    {
        var normalizedNewTagValues = tags
            .Select(_sm.UtilityService.NormalizeText)
            .ToList();

        foreach (var tagValue in normalizedNewTagValues)
        {
            var tag = await _tagRepository.GetByValueAsync(tagValue);

            if (tag == null)
            {
                tag = new Tag { Value = tagValue };
                await _tagRepository.AddAsync(tag);
            }

            if (!question.Tags.Any(t => t.Value == tagValue))
            {
                question.Tags.Add(tag);
            }
        }
    }
}

