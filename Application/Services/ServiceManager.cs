using AutoMapper;

namespace Application.Services;

public class ServiceManager(
    Lazy<IBaseService> baseService,
    Lazy<IQuestionService> questionService,
    Lazy<IAuthenticationService> authenticationService,
    IMapper mapper
) : IServiceManager
{
    public IBaseService BaseService => baseService.Value;
    public IQuestionService QuestionService => questionService.Value;

    public IAuthenticationService AuthenticationService => authenticationService.Value;
    public IMapper Mapper => mapper;
}


