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
    IMapper Mapper { get; }
    ITagService TagService { get; }
}
