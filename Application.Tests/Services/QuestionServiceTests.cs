using Application.Services;
using Application.Tests.Utilities;
using Domain.DTO.Query;
using Domain.DTO.Response;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Application.Tests.Services;

public class QuestionServiceTests : BaseServiceSetupTests
{
    private readonly Mock<Topic> _mockTopic;
    private readonly Mock<User> _mockUser;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly QuestionService _questionService;

    public QuestionServiceTests()
    {
        _mockTopic = new Mock<Topic>();

        _mockUser = new Mock<User>();

        _mockTokenService = new Mock<ITokenService>();

        _questionService = new QuestionService(
            _mockRepositoryManager.Object,
            _mockServiceManager.Object,
            _mockUserManager.Object);

        _mockServiceManager
            .Setup(sm => sm.TokenService)
            .Returns(_mockTokenService.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldSetAnsweredByTeacher_WhenTeacherAnswers()
    {
        // Arrange
        var questionId = Guid.NewGuid();
        var subjectId = Guid.NewGuid();
        var answerId = Guid.NewGuid();

        var question = QuestionFactory.CreateQuestionEntity(
            questionId, _mockTopic.Object, _mockUser.Object);

        var questionDTO = QuestionFactory.CreateQuestionDetailedDto(
            _mockTopic.Object.Id);

        _mockQuestionRepository
            .Setup(q => q.GetByIdAsync(questionId))
            .ReturnsAsync(question);

        _mockMapper
            .Setup(m => m.Map<QuestionDetailedDTO>(question))
            .Returns(questionDTO);

        _mockUserRepository
            .Setup(t => t.GetTeachersBySubjectIdAsync(subjectId))
            .ReturnsAsync([]);

        // Act
        var result = await _questionService.GetByIdAsync(questionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(result.Id, questionDTO.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnQuestion_WhenExists()
    {
        // Arrange
        var questionId = Guid.NewGuid();
        var subjectId = Guid.NewGuid();
        var answerId = Guid.NewGuid();

        var questionEntity = QuestionFactory.CreateQuestionEntity(
            questionId, _mockTopic.Object, _mockUser.Object);

        var questionDTO = QuestionFactory.CreateQuestionDetailedDto(
            _mockTopic.Object.Id);

        _mockQuestionRepository
            .Setup(q => q.GetByIdAsync(questionId))
            .ReturnsAsync(questionEntity);

        _mockMapper
            .Setup(m => m.Map<QuestionDetailedDTO>(questionEntity))
            .Returns(questionDTO);

        _mockTokenService
            .Setup(t => t.GetUserId())
            .Returns(_mockUser.Object.Id);

        _mockUserRepository
            .Setup(u => u.GetTeachersBySubjectIdAsync(subjectId))
            .ReturnsAsync([]);

        // Act
        var result = await _questionService.GetByIdAsync(questionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(questionDTO.Id, result.Id);
        // TODO Fix failing Question Id
        Assert.Equal(questionEntity.Id, result.Id);
    }


}
