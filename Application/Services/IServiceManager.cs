using AutoMapper;

namespace Application.Services;

public interface IServiceManager
{
    IBaseService BaseService { get; }
    IQuestionService QuestionService { get; }
    IAuthenticationService AuthenticationService{ get; }
    IMapper Mapper { get; }
}
