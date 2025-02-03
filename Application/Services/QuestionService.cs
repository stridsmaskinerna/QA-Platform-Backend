using System.Text;
using System.Text.RegularExpressions;
using Bogus.Bson;
using Domain.Contracts;
using Domain.DTO.Query;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Application.Services;

public class QuestionService : BaseService, IQuestionService
{
    private readonly IQuestionRepository _qr;
    private readonly ITagRepository _tr;
    private readonly IServiceManager _sm;

    public QuestionService(
        IQuestionRepository qr,
        ITagRepository tr,
        IServiceManager sm
    )
    {
        _qr = qr;
        _tr = tr;
        _sm = sm;
    }

    public async
    Task<(IEnumerable<Question> Questions, int TotalItemCount)>
    GetItemsAsync(
        PaginationDTO paginationDTO,
        QuestionSearchDTO searchDTO,
        bool onlyPublic = true
    )
    {
        return await _qr.GetItemsAsync(paginationDTO, searchDTO, onlyPublic);
    }

    public async Task<QuestionDetailedDTO> GetByIdAsync(Guid id)
    {
        var question = await _qr.GetByIdAsync(id);

        if (question == null)
        {
            NotFound();
        }

        return _sm.Mapper.Map<QuestionDetailedDTO>(question);
    }

    public async Task DeleteAsync(Guid id)
    {
        var question = await _qr.GetByIdAsync(id);

        if (question == null)
        {
            NotFound();
        }

        await _qr.DeleteAsync(id);
    }

    public async Task<QuestionDTO> AddAsync(QuestionForCreationDTO questionDTO)
    {
        var question = _sm.Mapper.Map<Question>(questionDTO);

        if (question == null)
        {
            BadRequest("Invalid Question body");
        }

        question.UserId = _sm.TokenService.GetUserId();

        await AddNewTags(question, questionDTO.Tags);

        var createdQuestion = await _qr.AddAsync(question);

        var createdQuestionDTO = _sm.Mapper.Map<QuestionDTO>(createdQuestion);

        createdQuestionDTO.UserName = _sm.TokenService.GetUserName();

        return createdQuestionDTO;
    }

    public async Task UpdateAsync(Guid id, QuestionForPutDTO questionDTO)
    {
        var question = await _qr.GetByIdAsync(id);
        if (question == null)
        {
            NotFound($"No answer with id {id} exist.");
        }

        var normalizedNewTagValues = questionDTO.Tags
            .Select(_sm.UtilityService.NormalizeText)
            .ToList();

        //var existingTagValues = question.Tags
        //    .Select(tag => tag.Value)
        //    .ToList();

        var tagsToRemove = question.Tags
            .Where(t => !normalizedNewTagValues.Contains(t.Value))
            .ToList();

        foreach (var tag in tagsToRemove)
        {
            question.Tags.Remove(tag);
        }

        await _qr.CompleteAsync();

        foreach (var tag in tagsToRemove)
        {
            await _tr.DeleteUnusedTagsAsync(tag);
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
            var tag = await _tr.GetByValueAsync(tagValue);

            if (tag == null)
            {
                tag = new Tag { Value = tagValue };
                await _tr.AddAsync(tag);
            }

            if (!question.Tags.Any(t => t.Value == tagValue))
            {
                question.Tags.Add(tag);
            }
        }
    }
}

