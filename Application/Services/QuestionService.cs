using Application.Contracts;
using Domain.Constants;
using Domain.Contracts;
using Domain.DTO.Query;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Services;

public class QuestionService : BaseService, IQuestionService
{
    private readonly IRepositoryManager _rm;
    private readonly IServiceManager _sm;

    public QuestionService(
        IRepositoryManager rm,
        IServiceManager sm,
        UserManager<User> userManager
    )
    {
        _rm = rm;
        _sm = sm;
    }

    public static string MsgNotFound(Guid id) => $"Question with id {id} does not exist.";

    public static string MsgBadRequest() => "Invalid question body";

    public async Task<(IEnumerable<QuestionDTO> Questions, int TotalItemCount, int ExcludedItems)> GetItemsAsync(
        PaginationDTO paginationDTO,
        QuestionSearchDTO searchDTO,
        bool onlyPublic = true
    )
    {
        string? userId = null;
        List<string>? userRoles = null;
        if (!onlyPublic)
        {
            userId = _sm.TokenService.GetUserId();
            userRoles = _sm.TokenService.GetUserRoles();
        }
        var (Questions, TotalItemCount) = await _rm.QuestionRepository.GetItemsAsync(
            paginationDTO, searchDTO, onlyPublic, userId, userRoles);

        var questionDTOs = _sm.Mapper.Map<IEnumerable<QuestionDTO>>(
            Questions);

        int ExcludedItems = 0;
        if (userId is not null && userRoles is not null && userRoles.Contains(DomainRoles.TEACHER))
        {
            await _sm.DTOService.UpdateQuestionIsHideableField(questionDTOs, userId);
            //Filter out all question that are hidden and not hideable
            questionDTOs = questionDTOs.Where(q => !q.IsHidden || q.IsHideable).ToList();
            ExcludedItems = Questions.Count() - questionDTOs.Count();

        }



        return (
            Questions: questionDTOs, TotalItemCount, ExcludedItems
        );
    }

    public async Task ManageQuestionVisibilityAsync(Guid id)
    {
        var question = await _rm.QuestionRepository.GetByIdAsync(id);
        if (question == null)
        {
            NotFound(MsgNotFound(id));
        }
        question.IsHidden = !question.IsHidden;
        await _rm.QuestionRepository.UpdateAsync(question);
    }

    public async Task<(IEnumerable<QuestionDTO> Questions, int TotalItemCount)> GetTeacherQuestionsAsync(
        PaginationDTO paginationDTO,
        Guid subjectId
    )
    {
        var userId = _sm.TokenService.GetUserId();

        var teachers = await _rm.UserRepository.GetTeachersBySubjectIdAsync(subjectId);

        var teacher = teachers
            .Where(t => t.Id == userId)
            .FirstOrDefault();

        if (teacher == null)
        {
            BadRequest($"User is not teachers for subject with id {subjectId}.");
        }

        var questionWithItemCount = await _rm.QuestionRepository.GetTeacherQuestionsAsync(
            paginationDTO, subjectId, teacher);

        var questionDTOList = _sm.Mapper.Map<IEnumerable<QuestionDTO>>(
            questionWithItemCount.Questions);

        foreach (var dto in questionDTOList)
        {
            dto.IsHideable = true;
        }


        return (
            Questions: questionDTOList,
            TotalItemCount: questionWithItemCount.TotalItemCount
        );
    }

    public async Task<QuestionDetailedDTO> GetByIdAsync(Guid id)
    {
        var question = await _rm.QuestionRepository.GetByIdAsync(id);

        if (question == null)
        {
            NotFound(MsgNotFound(id));
        }

        var questionDTO = _sm.Mapper.Map<QuestionDetailedDTO>(question);

        var userId = _sm.TokenService.GetUserId();

        await _sm.DTOService.UpdatingAnsweredByTeacherField(questionDTO);

        _sm.DTOService.UpdatingMyVoteField(question, questionDTO, userId);

        await _sm.DTOService.UpdateQuestionIsHideableField(questionDTO, userId);

        _sm.DTOService.UpdateAnswerIsHideableField(questionDTO);

        if (questionDTO.IsHidden && !questionDTO.IsHideable)
        {
            NotFound(MsgNotFound(id));
        }

        return questionDTO;
    }

    public async Task<QuestionDetailedDTO> GetPublicQuestionByIdAsync(Guid id)
    {
        var question = await _rm.QuestionRepository.GetByIdAsync(id);

        // If question is protected send NotFound
        // to not provide any information to unauthenticated users.
        if (question == null || question.IsProtected || question.IsHidden)
        {
            NotFound(MsgNotFound(id));
        }

        var questionDTO = _sm.Mapper.Map<QuestionDetailedDTO>(question);

        await _sm.DTOService.UpdatingAnsweredByTeacherField(questionDTO);

        return questionDTO;
    }

    public async Task DeleteAsync(Guid id)
    {
        var question = await _rm.QuestionRepository.GetByIdAsync(id);

        if (question == null)
        {
            NotFound(MsgNotFound(id));
        }

        await _rm.QuestionRepository.DeleteAsync(id);
    }

    public async Task<QuestionDTO> AddAsync(QuestionForCreationDTO questionDTO)
    {
        var question = _sm.Mapper.Map<Question>(questionDTO);

        var topic = await _rm.TopicRepository.GetByIdAsync(questionDTO.TopicId);

        if (question == null || topic == null)
        {
            BadRequest(MsgBadRequest());
        }

        question.UserId = _sm.TokenService.GetUserId();

        await _sm.TagService.StoreNewTagsFromQuestion(question, questionDTO.Tags);

        var createdQuestion = await _rm.QuestionRepository.AddAsync(question);

        var createdQuestionDTO = _sm.Mapper.Map<QuestionDTO>(createdQuestion);

        createdQuestionDTO.UserName = _sm.TokenService.GetUserName();

        return createdQuestionDTO;
    }

    public async Task UpdateAsync(Guid id, QuestionForPutDTO questionDTO)
    {
        var question = await _rm.QuestionRepository.GetByIdAsync(id);

        _sm.Mapper.Map(questionDTO, question);

        if (question == null)
        {
            NotFound(MsgNotFound(id));
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

        await _rm.QuestionRepository.CompleteAsync();

        await _rm.TagRepository.DeleteUnusedTagsAsync(tagsToRemove);

        await _sm.TagService.StoreNewTagsFromQuestion(question, questionDTO.Tags);

        await _rm.QuestionRepository.UpdateAsync(question);
    }
}

