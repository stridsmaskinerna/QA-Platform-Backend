using AutoMapper;

namespace Application.Services;

public interface IServiceManager
{
    IBaseService BaseService { get; }
    IQuestionService QuestionService { get; }
    IAnswerService AnswerService { get; }
    IAuthenticationService AuthenticationService { get; }
    ITokenService TokenService { get; }
    IMapper Mapper { get; }
    ITagService TagService { get; }
}
