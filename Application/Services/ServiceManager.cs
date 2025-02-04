using AutoMapper;

namespace Application.Services;

public class ServiceManager(
    Lazy<IBaseService> baseService,
    Lazy<IQuestionService> questionService,
    Lazy<IAnswerService> answerService,
    Lazy<ICommentService> commentService,
    Lazy<IAuthenticationService> authenticationService,
    Lazy<ITokenService> tokenService,
    IMapper mapper
) : IServiceManager
{
    public IBaseService BaseService => baseService.Value;
    public IQuestionService QuestionService => questionService.Value;
    public IAuthenticationService AuthenticationService => authenticationService.Value;
    public ITokenService TokenService => tokenService.Value;
    public IMapper Mapper => mapper;
    public IAnswerService AnswerService => answerService.Value;
    public ICommentService CommentService => commentService.Value;
}


