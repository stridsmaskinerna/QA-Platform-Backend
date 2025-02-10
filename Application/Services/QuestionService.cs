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

    public async Task<(IEnumerable<QuestionDTO> Questions, int TotalItemCount)> GetItemsAsync(
        PaginationDTO paginationDTO,
        QuestionSearchDTO searchDTO,
        bool onlyPublic = true
    )
    {
        string? userId = null;
        if (!onlyPublic) userId = _sm.TokenService.GetUserId();

        var questionWithItemCount = await _rm.QuestionRepository.GetItemsAsync(
            paginationDTO, searchDTO, onlyPublic, userId);

        var questionDTOList = _sm.Mapper.Map<IEnumerable<QuestionDTO>>(
            questionWithItemCount.Questions);

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

        await UpdatingAnsweredByTeacherField(questionDTO);

        UpdatingMyVoteField(question, questionDTO);

        return questionDTO;
    }

    public async Task<QuestionDetailedDTO> GetPublicQuestionByIdAsync(Guid id)
    {
        var question = await _rm.QuestionRepository.GetByIdAsync(id);

        // If question is protected send NotFound
        // to not provide any information to unauthenticated users.
        if (question == null || question.IsProtected)
        {
            NotFound(MsgNotFound(id));
        }

        var questionDTO = _sm.Mapper.Map<QuestionDetailedDTO>(question);

        await UpdatingAnsweredByTeacherField(questionDTO);

        return questionDTO;
    }

    private async Task UpdatingAnsweredByTeacherField(QuestionDetailedDTO questionDTO)
    {
        var answersUsernames = questionDTO.Answers?
            .Select(a => a.UserName)
            .ToList();

        var teachers = await _rm.UserRepository.GetTeachersBySubjectIdAsync(questionDTO.SubjectId);

        var teachersUsernames = teachers.Select(u => u.UserName).ToList();

        foreach (var answer in questionDTO.Answers ?? [])
        {
            if (teachersUsernames.Contains(answer.UserName))
            {
                answer.AnsweredByTeacher = true;
            }
        }
    }

    private void UpdatingMyVoteField(
        Question question,
        QuestionDetailedDTO questionDTO
    )
    {
        var userId = _sm.TokenService.GetUserId();

        var answerVotesDict = (question.Answers ?? Enumerable.Empty<Answer>())
            .SelectMany(a => a.AnswerVotes)
            .Where(v => v.UserId == userId)
            .ToDictionary(v => v.AnswerId, v => v.Vote);

        foreach (var answerDTO in questionDTO.Answers ?? [])
        {
            if (answerVotesDict.TryGetValue(answerDTO.Id, out bool vote))
            {
                answerDTO.MyVote = vote switch
                {
                    true => VoteType.LIKE,
                    false => VoteType.DISLIKE
                };
            }
            else
            {
                answerDTO.MyVote = VoteType.NEUTRAL;
            }
        }
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

        await AddNewTags(question, questionDTO.Tags);

        var createdQuestion = await _rm.QuestionRepository.AddAsync(question);

        var createdQuestionDTO = _sm.Mapper.Map<QuestionDTO>(createdQuestion);

        createdQuestionDTO.UserName = _sm.TokenService.GetUserName();

        return createdQuestionDTO;
    }

    public async Task UpdateAsync(Guid id, QuestionForPutDTO questionDTO)
    {
        var question = await _rm.QuestionRepository.GetByIdAsync(id);

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

