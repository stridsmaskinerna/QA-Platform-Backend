using Application;
using Application.Contracts;
using Application.Services;
using Domain.Contracts;
using Infrastructure.Repositories;

namespace QAPlatformAPI.Extensions;

public static class ApplicationServicesExtension
{
    public static void AddApplicationServicesExtension(this IServiceCollection services)
    {
        // Context Accessors
        services.AddHttpContextAccessor();

        // Mapper
        services.AddAutoMapper(typeof(ApplicationAssembly).Assembly);

        // Managers
        services.AddScoped<IServiceManager, ServiceManager>();
        services.AddScoped<IRepositoryManager, RepositoryManager>();

        // Services
        services.AddAsLazy<IAnswerService, AnswerService>();
        services.AddAsLazy<IAuthenticationService, AuthenticationService>();
        services.AddAsLazy<IBaseService, BaseService>();
        services.AddAsLazy<ICommentService, CommentService>();
        services.AddAsLazy<IQuestionService, QuestionService>();
        services.AddAsLazy<ISubjectService, SubjectService>();
        services.AddAsLazy<ITagService, TagService>();
        services.AddAsLazy<ITokenService, TokenService>();
        services.AddAsLazy<ITopicService, TopicService>();
        services.AddAsLazy<ITeacherService, TeacherService>();
        services.AddAsLazy<IDTOService, DTOService>();
        services.AddAsLazy<IUtilityService, UtilityService>();
        services.AddAsLazy<IVoteService, VoteService>();

        // Repositories
        services.AddAsLazy<IAnswerRepository, AnswerRepository>();
        services.AddAsLazy<IAnswerVoteRepository, AnswerVoteRepository>();
        services.AddAsLazy<ICommentRepository, CommentRepository>();
        services.AddAsLazy<IQuestionRepository, QuestionRepository>();
        services.AddAsLazy<ISubjectRepository, SubjectRepository>();
        services.AddAsLazy<ITagRepository, TagRepository>();
        services.AddAsLazy<ITopicRepository, TopicRepository>();
        services.AddAsLazy<IUserRepository, UserRepository>();
    }

    private static void AddAsLazy<IServiceType, ServiceType>(
        this IServiceCollection collection,
        ServiceLifetime lifetime = ServiceLifetime.Scoped
    )
        where ServiceType : class, IServiceType
        where IServiceType : class
    {
        collection.Add(new ServiceDescriptor(
            typeof(IServiceType),
            typeof(ServiceType),
            lifetime
        ));

        collection.Add(new ServiceDescriptor(
            typeof(Lazy<IServiceType>),
            p => new Lazy<IServiceType>(() => p.GetRequiredService<IServiceType>()),
            lifetime
        ));
    }
}
