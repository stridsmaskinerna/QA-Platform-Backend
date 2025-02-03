using AutoMapper;

namespace Application.Services;

public class ServiceManager(
    Lazy<IBaseService> baseService,
    Lazy<IQuestionService> questionService,
    Lazy<IAnswerService> answerService,
    Lazy<IAuthenticationService> authenticationService,
    Lazy<ITokenService> tokenService,
    Lazy<ITagService> tagService,
    Lazy<IUtilityService> utilityService,
    IMapper mapper
) : IServiceManager
{
    public IBaseService BaseService => baseService.Value;
    public IQuestionService QuestionService => questionService.Value;
    public IAuthenticationService AuthenticationService => authenticationService.Value;
    public ITokenService TokenService => tokenService.Value;
    public IMapper Mapper => mapper;
    public IAnswerService AnswerService => answerService.Value;
    public ITagService TagService => tagService.Value;
    public IUtilityService UtilityService => utilityService.Value;
}


