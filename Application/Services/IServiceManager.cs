using AutoMapper;

namespace Application.Services;

public interface IServiceManager
{
    IBaseService BaseService { get; }
    IQuestionService QuestionService { get; }
    IMapper Mapper { get; }
}
