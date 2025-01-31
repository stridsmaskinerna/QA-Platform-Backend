using AutoMapper;
using Infrastructure.Contexts;

namespace Application.Services;

public class ServiceManager(
    Lazy<IBaseService> baseService,
    Lazy<IQuestionService> questionService,
    Lazy<IAuthenticationService> authenticationService,
    Lazy<ITokenService> tokenService,
    IMapper mapper,
    QAPlatformContext context
) : IServiceManager
{
    public IBaseService BaseService => baseService.Value;
    public IQuestionService QuestionService => questionService.Value;
    public IAuthenticationService AuthenticationService => authenticationService.Value;
    public ITokenService TokenService => tokenService.Value;
    public IMapper Mapper => mapper;
    public QAPlatformContext Context => context;
}


