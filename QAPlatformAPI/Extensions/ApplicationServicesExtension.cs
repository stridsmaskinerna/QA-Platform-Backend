using Application;
using Application.Contracts;
using Application.Services;
using Domain.Contracts;
using Infrastructure.Repositories;

namespace QAPlatformAPI.Extensions;

public static class ApplicationServicesExtension
{
    public static void AddApplicationServicesExtension(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddAutoMapper(typeof(ApplicationAssembly).Assembly);

        builder.Services.AddScoped<IServiceManager, ServiceManager>();
        builder.Services.AddAsLazy<IBaseService, BaseService>();
        builder.Services.AddAsLazy<IQuestionService, QuestionService>();
        builder.Services.AddAsLazy<IAnswerService, AnswerService>();
        builder.Services.AddAsLazy<IAuthenticationService, AuthenticationService>();
        builder.Services.AddAsLazy<ITokenService, TokenService>();
        builder.Services.AddAsLazy<ICommentService, CommentService>();
        builder.Services.AddAsLazy<ITagService, TagService>();
        builder.Services.AddAsLazy<IUtilityService, UtilityService>();
        builder.Services.AddAsLazy<IVoteService, VoteService>();

        builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
        builder.Services.AddAsLazy<IQuestionRepository, QuestionRepository>();
        builder.Services.AddAsLazy<IAnswerRepository, AnswerRepository>();
        builder.Services.AddAsLazy<ICommentRepository, CommentRepository>();
        builder.Services.AddAsLazy<ISubjectRepository, SubjectRepository>();
        builder.Services.AddAsLazy<ITopicRepository, TopicRepository>();
        builder.Services.AddAsLazy<ITagRepository, TagRepository>();
        builder.Services.AddAsLazy<IAnswerVoteRepository, AnswerVoteRepository>();
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
