using AutoMapper;

namespace Application.Services;

public class ServiceManager(
    Lazy<IBaseService> baseService, Lazy<IQuestionService> questionService, IMapper mapper
) : IServiceManager
{
    public IBaseService BaseService => baseService.Value;
    public IQuestionService QuestionService => questionService.Value;
    public IMapper Mapper => mapper;
}


