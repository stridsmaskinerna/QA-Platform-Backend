using Application.Contracts;
using Domain.Constants;
using Domain.Contracts;
using Domain.DTO.Response;
using Domain.Entities;

namespace Application.Services;

public class DTOService : IDTOService
{
    private readonly IRepositoryManager _rm;

    public DTOService(IRepositoryManager rm)
    {
        _rm = rm;
    }

    public async Task UpdateQuestionIsHideableField(IEnumerable<QuestionDTO> DTOList, string userId)
    {
        var teachersSubjectIds = (await _rm.SubjectRepository.GetTeachersSubjectsAsync(userId)).Select(s => s.Id);

        foreach (var dto in DTOList)
        {
            dto.IsHideable = teachersSubjectIds.Contains(dto.SubjectId);
        }
    }

    public async Task UpdateQuestionIsHideableField(QuestionDTO dto, string userId)
    {
        var teachersSubjectIds = (await _rm.SubjectRepository.GetTeachersSubjectsAsync(userId)).Select(s => s.Id);

        dto.IsHideable = teachersSubjectIds.Contains(dto.SubjectId);
    }

    public void UpdateAnswerIsHideableField(QuestionDetailedDTO questionDTO)
    {
        if (!questionDTO.IsHideable)
        {
            return;
        }

        foreach (var answerDTO in questionDTO.Answers ?? [])
        {
            answerDTO.IsHideable = true;
        }
    }

    public async Task UpdatingAnsweredByTeacherField(QuestionDetailedDTO questionDTO)
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

    public void UpdatingMyVoteField(
       Question question,
       QuestionDetailedDTO questionDTO,
       string? userId
    )
    {
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
}
