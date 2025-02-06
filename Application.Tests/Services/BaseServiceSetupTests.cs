using Application.Contracts;
using AutoMapper;
using Domain.Contracts;
using Domain.Entities;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Application.Tests.Services;

public class BaseServiceSetupTests
{
    protected readonly Mock<IRepositoryManager> _mockRepositoryManager;
    protected readonly Mock<IServiceManager> _mockServiceManager;

    protected readonly Mock<IAnswerRepository> _mockAnswerRepository;
    protected readonly Mock<IAnswerVoteRepository> _mockAnswerVoteRepository;
    protected readonly Mock<ICommentRepository> _mockCommentRepository;
    protected readonly Mock<IQuestionRepository> _mockQuestionRepository;
    protected readonly Mock<ISubjectRepository> _mockSubjectRepository;
    protected readonly Mock<ITagRepository> _mockTagRepository;
    protected readonly Mock<ITopicRepository> _mockTopicRepository;
    protected readonly Mock<IUserRepository> _mockUserRepository;

    protected readonly Mock<IMapper> _mockMapper;
    protected readonly Mock<UserManager<User>> _mockUserManager;

    public BaseServiceSetupTests()
    {
        // Mock repositories
        _mockAnswerRepository = new Mock<IAnswerRepository>();
        _mockAnswerVoteRepository = new Mock<IAnswerVoteRepository>();
        _mockCommentRepository = new Mock<ICommentRepository>();
        _mockQuestionRepository = new Mock<IQuestionRepository>();
        _mockSubjectRepository = new Mock<ISubjectRepository>();
        _mockTagRepository = new Mock<ITagRepository>();
        _mockTopicRepository = new Mock<ITopicRepository>();
        _mockUserRepository = new Mock<IUserRepository>();

        // Mock General services
        _mockMapper = new Mock<IMapper>();
        _mockUserManager = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(),
            null!, null!, null!, null!, null!, null!, null!, null!
        );

        // Mock Repository manager
        _mockRepositoryManager = new Mock<IRepositoryManager>();

        _mockRepositoryManager
            .Setup(rm => rm.AnswerRepository)
            .Returns(_mockAnswerRepository.Object);

        _mockRepositoryManager
             .Setup(rm => rm.AnswerVoteRepository)
            .Returns(_mockAnswerVoteRepository.Object);

        _mockRepositoryManager
            .Setup(rm => rm.CommentRepository)
            .Returns(_mockCommentRepository.Object);

        _mockRepositoryManager
            .Setup(rm => rm.QuestionRepository)
            .Returns(_mockQuestionRepository.Object);

        _mockRepositoryManager
            .Setup(rm => rm.SubjectRepository)
            .Returns(_mockSubjectRepository.Object);

        _mockRepositoryManager
            .Setup(rm => rm.TagRepository)
            .Returns(_mockTagRepository.Object);

        _mockRepositoryManager
            .Setup(rm => rm.TopicRepository)
            .Returns(_mockTopicRepository.Object);

        _mockRepositoryManager
            .Setup(rm => rm.UserRepository)
            .Returns(_mockUserRepository.Object);

        // Mock Service manager
        _mockServiceManager = new Mock<IServiceManager>();

        _mockServiceManager
            .Setup(sm => sm.Mapper)
            .Returns(_mockMapper.Object);
    }
}
