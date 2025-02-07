using AutoMapper;

namespace Application.Contracts;

public interface IServiceManager
{
    IBaseService BaseService { get; }
    IQuestionService QuestionService { get; }
    IAnswerService AnswerService { get; }
    ICommentService CommentService { get; }
    IAuthenticationService AuthenticationService { get; }
    ITokenService TokenService { get; }
    IUtilityService UtilityService { get; }
    ITagService TagService { get; }
    IVoteService VoteService { get; }
    ISubjectService SubjectService { get; }
    IMapper Mapper { get; }
}
