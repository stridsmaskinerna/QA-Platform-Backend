using Application.Services;
using Application.Tests.Utilities;
using Domain.Constants;
using Domain.DTO.Response;
using Domain.Entities;
using Domain.Exceptions;
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

    public class GetByIdAsync : QuestionServiceTests
    {
        [Fact]
        public async Task ShouldReturnDetailedQuestion_WhenExists()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            var subjectId = Guid.NewGuid();
            var answerId = Guid.NewGuid();

            var questionEntity = QuestionFactory.CreateQuestionEntity(
                questionId, _mockTopic.Object, _mockUser.Object);

            var questionDTO = QuestionFactory.CreateQuestionDetailedDto(
                questionId, _mockTopic.Object.Id);

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
            Assert.Equal(questionEntity.Id, result.Id);
        }

        [Fact]
        public async Task ShouldThrowNotFound_WhenQuestionDoesNotExist()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            _mockQuestionRepository
                .Setup(q => q.GetByIdAsync(questionId))
                .ReturnsAsync(default(Question));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => _questionService.GetByIdAsync(questionId));

            Assert.Equal(
                _questionService.MsgNotFound(questionId),
                exception.Message);
        }

        [Fact]
        public async Task ShouldSetAnsweredByTeacher_WhenTeacherAnswers()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            var answerId = Guid.NewGuid();

            var teacherUser = UserFactory.CreateUser("teacher123", "teacher-userName");

            var questionEntity = QuestionFactory.CreateQuestionEntity(
                questionId, _mockTopic.Object, _mockUser.Object);

            questionEntity.Answers = [AnswerFactory.CreateAnswerEntity(
            answerId, questionEntity, teacherUser)];

            var questionDTO = QuestionFactory.CreateQuestionDetailedDto(
                questionId, _mockTopic.Object.Id);

            questionDTO.Answers = [AnswerFactory.CreateAnswerDetailedDTO(
            questionEntity.Answers.First(), teacherUser.UserName ?? String.Empty)];

            questionDTO.SubjectId = Guid.NewGuid();

            _mockQuestionRepository
                .Setup(q => q.GetByIdAsync(questionId))
                .ReturnsAsync(questionEntity);

            _mockMapper
                .Setup(m => m.Map<QuestionDetailedDTO>(questionEntity))
                .Returns(questionDTO);

            _mockUserRepository
                .Setup(u => u.GetTeachersBySubjectIdAsync(questionDTO.SubjectId))
                .ReturnsAsync([teacherUser]);

            // Act
            var result = await _questionService.GetByIdAsync(questionId);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Answers);

            Assert.Equal(result.Answers, questionDTO.Answers);
            Assert.True(result.Answers.First().AnsweredByTeacher);
        }

        [Theory]
        [InlineData(VoteType.DISLIKE, false)]
        [InlineData(VoteType.LIKE, true)]
        [InlineData(VoteType.NEUTRAL, null)]
        public async Task ShouldSetMyVoteField_WhenUsersHaveVoted(
            string expectedVoteValue, bool? voteValueAsBoolean
        )
        {
            // Arrange
            var questionId = Guid.NewGuid();
            var userId = "user123";
            var answerId = Guid.NewGuid();

            var user = UserFactory.CreateUser("user123", "test userName");

            var questionEntity = QuestionFactory.CreateQuestionEntity(
                questionId, _mockTopic.Object, _mockUser.Object);

            questionEntity.Answers = [AnswerFactory.CreateAnswerEntity(
            answerId, questionEntity, user)];

            if (voteValueAsBoolean != null)
            {
                questionEntity.Answers.First().AnswerVotes = [
                AnswerVoteFactory.CreateAnswerVoteEntity(
                answerId, user, questionEntity.Answers.First(), voteValueAsBoolean.Value)
                ];
            }

            var questionDTO = QuestionFactory.CreateQuestionDetailedDto(
                questionId, _mockTopic.Object.Id);

            questionDTO.Answers = [AnswerFactory.CreateAnswerDetailedDTO(
            questionEntity.Answers.First(), user.UserName ?? String.Empty)];

            _mockQuestionRepository
                .Setup(q => q.GetByIdAsync(questionId))
                .ReturnsAsync(questionEntity);

            _mockMapper
                .Setup(m => m.Map<QuestionDetailedDTO>(questionEntity))
                .Returns(questionDTO);

            _mockTokenService
                .Setup(t => t.GetUserId())
                .Returns(userId);

            _mockUserRepository
                .Setup(u => u.GetTeachersBySubjectIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync([]);

            // Act
            var result = await _questionService.GetByIdAsync(questionId);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Answers);

            Assert.Equal(expectedVoteValue, result.Answers.First().MyVote);
        }

    }

    public class DeleteAsync : QuestionServiceTests
    {
        [Fact]
        public async Task ShouldDeleteQuestion_WhenExists()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            var question = QuestionFactory.CreateQuestionEntity(
                questionId, _mockTopic.Object, _mockUser.Object);

            _mockQuestionRepository
                .Setup(q => q.GetByIdAsync(questionId))
                .ReturnsAsync(question);

            // Act
            await _questionService.DeleteAsync(questionId);

            // Assert
            _mockQuestionRepository.Verify(
                q => q.DeleteAsync(questionId),
                Times.Once);
        }

        [Fact]
        public async Task ShouldThrowNotFound_WhenQuestionDoesNotExist()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            _mockQuestionRepository
                .Setup(q => q.GetByIdAsync(questionId))
                .ReturnsAsync(default(Question));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => _questionService.DeleteAsync(questionId));

            Assert.Equal(_questionService.MsgNotFound(questionId), exception.Message);
        }

        [Fact]
        public async Task ShouldNotCallDelete_WhenQuestionDoesNotExist()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            _mockQuestionRepository
                .Setup(q => q.GetByIdAsync(questionId))
                .ReturnsAsync(default(Question));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => _questionService.DeleteAsync(questionId));

            _mockQuestionRepository.Verify(
                q => q.DeleteAsync(questionId),
                Times.Never);
        }
    }
}
