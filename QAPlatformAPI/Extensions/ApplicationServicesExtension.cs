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
        services.AddHttpContextAccessor();

        services.AddAutoMapper(typeof(ApplicationAssembly).Assembly);

        services.AddScoped<IServiceManager, ServiceManager>();
        services.AddAsLazy<IBaseService, BaseService>();
        services.AddAsLazy<IQuestionService, QuestionService>();
        services.AddAsLazy<IAnswerService, AnswerService>();
        services.AddAsLazy<IAuthenticationService, AuthenticationService>();
        services.AddAsLazy<ITokenService, TokenService>();
        services.AddAsLazy<ICommentService, CommentService>();
        services.AddAsLazy<ITagService, TagService>();
        services.AddAsLazy<IUtilityService, UtilityService>();
        services.AddAsLazy<ISubjectService, SubjectService>();
        services.AddAsLazy<IVoteService, VoteService>();

        services.AddScoped<IRepositoryManager, RepositoryManager>();
        services.AddAsLazy<IQuestionRepository, QuestionRepository>();
        services.AddAsLazy<IAnswerRepository, AnswerRepository>();
        services.AddAsLazy<ICommentRepository, CommentRepository>();
        services.AddAsLazy<ISubjectRepository, SubjectRepository>();
        services.AddAsLazy<ITopicRepository, TopicRepository>();
        services.AddAsLazy<ITagRepository, TagRepository>();
        services.AddAsLazy<IUserRepository, UserRepository>();
        services.AddAsLazy<IAnswerVoteRepository, AnswerVoteRepository>();
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
