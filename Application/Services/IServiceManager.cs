using AutoMapper;

namespace Application.Services;

public interface IServiceManager
{
    IBaseService BaseService { get; }
    IQuestionService QuestionService { get; }
    IAnswerService AnswerService { get; }
    ICommentService CommentService { get; }
    IAuthenticationService AuthenticationService { get; }
    ITokenService TokenService { get; }
    IMapper Mapper { get; }
}
