using Application.Services;
using Domain.Constants;
using Domain.Entities;
using Domain.Exceptions;
using Moq;
using TestUtility.Factories;

namespace Application.Tests.Services;

public class VoteServiceTests : SetupServiceTests
{
    private readonly VoteService _voteService;
    private readonly Mock<Question> _mockQuestion;
    private readonly Mock<User> _mockUser;
    private readonly Mock<ITokenService> _mockTokenService;

    public VoteServiceTests()
    {
        _mockQuestion = new Mock<Question>();

        _mockUser = new Mock<User>();

        _mockTokenService = new Mock<ITokenService>();

        _voteService = new VoteService(
            _mockRepositoryManager.Object,
            _mockServiceManager.Object,
            _mockUserManager.Object);

        _mockServiceManager
            .Setup(sm => sm.TokenService)
            .Returns(_mockTokenService.Object);
    }

    public class GetVoteAsBoolean : VoteServiceTests
    {
        [Theory]
        [InlineData(VoteType.LIKE, true)]
        [InlineData(VoteType.DISLIKE, false)]
        [InlineData("AnInvalidVoteType", null)]
        public void ShouldMapExpectedResult(
            string vote,
            bool? expected
        )
        {
            if (!VoteType.ALL_TYPES.Contains(vote))
            {
                var exception = Assert.Throws<BadRequestException>(
                    () => _voteService.GetVoteAsBoolean(vote));

                Assert.Equal(
                    VoteService.MsgInvalidVoteType(),
                    exception.Message);
            }
            else
            {
                var result = _voteService.GetVoteAsBoolean(vote);
                Assert.Equal(expected, result);
            }
        }
    }

    public class CastVoteAsync : VoteServiceTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [InlineData(null)]
        public async Task ShouldThrowNotFound_WhenAnswerDoesNotExist(
            bool? voteTypeAsBoolean
        )
        {
            // Arrange
            var answerId = Guid.NewGuid();
            _mockAnswerRepository
                .Setup(r => r.GetByIdAsync(answerId))
                .ReturnsAsync(default(Answer));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => _voteService.CastVoteAsync(answerId, voteTypeAsBoolean));

            Assert.Equal(
                VoteService.MsgCastVoteAnswerNotFound(answerId),
                exception.Message);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [InlineData(null)]
        public async Task ShouldNotCastAnyVote_WhenAnswerDoesNotExist(
            bool? voteTypeAsBoolean
        )
        {
            // Arrange
            var answerId = Guid.NewGuid();
            _mockAnswerRepository
                .Setup(r => r.GetByIdAsync(answerId))
                .ReturnsAsync(default(Answer));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => _voteService.CastVoteAsync(answerId, voteTypeAsBoolean));

            _mockAnswerVoteRepository.Verify(
                r => r.AddAsync(It.IsAny<AnswerVotes>()),
                Times.Never);

            _mockAnswerVoteRepository.Verify(
                r => r.UpdateAsync(It.IsAny<AnswerVotes>()),
                Times.Never);

            _mockAnswerVoteRepository.Verify(
                r => r.DeleteAsync(answerId, _mockUser.Object.Id),
                Times.Never);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [InlineData(null)]
        public async Task ShouldThrowUnauthorized_WhenUserNotFound(
            bool? voteTypeAsBoolean
        )
        {
            // Arrange
            var userId = "user123";

            var answerId = Guid.NewGuid();

            var answerEntity = AnswerFactory.CreateAnswerEntity(
                answerId, _mockQuestion.Object, _mockUser.Object);

            _mockAnswerRepository
                .Setup(r => r.GetByIdAsync(answerId))
                .ReturnsAsync(answerEntity);

            _mockTokenService
                .Setup(t => t.GetUserId())
                .Returns(userId);

            _mockUserManager
                .Setup(u => u.FindByIdAsync(userId))
                .ReturnsAsync(default(User));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedException>(
                () => _voteService.CastVoteAsync(answerId, voteTypeAsBoolean));

            Assert.Equal(
                VoteService.MsgCastVoteUserUnauthorized(),
                exception.Message);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [InlineData(null)]
        public async Task ShouldNotCastAnyVote_WhenUserNotFound(
            bool? voteTypeAsBoolean
        )
        {
            // Arrange
            var userId = "user123";

            var answerId = Guid.NewGuid();

            var answerEntity = AnswerFactory.CreateAnswerEntity(
                answerId, _mockQuestion.Object, _mockUser.Object);

            _mockAnswerRepository
                .Setup(r => r.GetByIdAsync(answerId))
                .ReturnsAsync(answerEntity);

            _mockTokenService
                .Setup(t => t.GetUserId())
                .Returns(userId);

            _mockUserManager
                .Setup(u => u.FindByIdAsync(userId))
                .ReturnsAsync(default(User));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedException>(
                () => _voteService.CastVoteAsync(answerId, voteTypeAsBoolean));

            _mockAnswerVoteRepository.Verify(
                r => r.AddAsync(It.IsAny<AnswerVotes>()),
                Times.Never);

            _mockAnswerVoteRepository.Verify(
                r => r.UpdateAsync(It.IsAny<AnswerVotes>()),
                Times.Never);

            _mockAnswerVoteRepository.Verify(
                r => r.DeleteAsync(answerId, _mockUser.Object.Id),
                Times.Never);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [InlineData(null)]
        public async Task ShouldCreateNewVote_WhenNoExistingVote_AndNewVoteIsNotNull(
            bool? voteAsBoolean
        )
        {
            // Arrange
            var answerId = Guid.NewGuid();
            var answerEntity = AnswerFactory.CreateAnswerEntity(
                answerId, _mockQuestion.Object, _mockUser.Object);

            _mockAnswerRepository
                .Setup(r => r.GetByIdAsync(answerId))
                .ReturnsAsync(answerEntity);

            _mockTokenService
                .Setup(t => t.GetUserId())
                .Returns(_mockUser.Object.Id);

            _mockUserManager
                .Setup(u => u.FindByIdAsync(_mockUser.Object.Id))
                .ReturnsAsync(_mockUser.Object);

            _mockAnswerVoteRepository
                .Setup(r => r.GetAsync(answerId, _mockUser.Object.Id))
                .ReturnsAsync(default(AnswerVotes));

            _mockAnswerVoteRepository
                .Setup(r => r.AddAsync(It.IsAny<AnswerVotes>()))
                .ReturnsAsync(It.IsAny<AnswerVotes>());

            // Act
            await _voteService.CastVoteAsync(answerId, voteAsBoolean);

            // Assert
            if (voteAsBoolean == null)
            {
                _mockAnswerVoteRepository.Verify(
                    r => r.AddAsync(It.IsAny<AnswerVotes>()),
                    Times.Never);
            }
            else
            {
                _mockAnswerVoteRepository.Verify(
                    r => r.AddAsync(It.IsAny<AnswerVotes>()),
                    Times.Once);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ShouldUpdateVote_WhenExistingVote_AndNewVoteIsNotNull(
            bool voteAsBoolean
        )
        {
            // Arrange
            var answerId = Guid.NewGuid();

            var answerEntity = AnswerFactory.CreateAnswerEntity(
                answerId, _mockQuestion.Object, _mockUser.Object);

            var existingVote = AnswerVoteFactory.CreateAnswerVoteEntity(
                answerId, _mockUser.Object, answerEntity);

            _mockAnswerRepository
                .Setup(r => r.GetByIdAsync(answerId))
                .ReturnsAsync(answerEntity);

            _mockTokenService
                .Setup(t => t.GetUserId())
                .Returns(_mockUser.Object.Id);

            _mockUserManager
                .Setup(u => u.FindByIdAsync(_mockUser.Object.Id))
                .ReturnsAsync(_mockUser.Object);

            _mockAnswerVoteRepository
                .Setup(r => r.GetAsync(answerId, _mockUser.Object.Id))
                .ReturnsAsync(existingVote);

            _mockAnswerVoteRepository
                .Setup(r => r.UpdateAsync(It.IsAny<AnswerVotes>()))
                .Returns(Task.CompletedTask);

            // Act
            await _voteService.CastVoteAsync(answerId, voteAsBoolean);

            // Assert
            Assert.Equal(voteAsBoolean, existingVote.Vote);
            _mockAnswerVoteRepository.Verify(
                r => r.UpdateAsync(existingVote),
                Times.Once);
        }

        [Fact]
        public async Task ShouldDeleteVote_WhenExistingVote_and_NewVoteIsNull()
        {
            // Arrange
            bool? voteValue = null;

            var answerId = Guid.NewGuid();

            var answerEntity = AnswerFactory.CreateAnswerEntity(
                answerId, _mockQuestion.Object, _mockUser.Object);

            var existingVote = AnswerVoteFactory.CreateAnswerVoteEntity(
                answerId, _mockUser.Object, answerEntity);

            _mockAnswerRepository
                .Setup(r => r.GetByIdAsync(answerId))
                .ReturnsAsync(answerEntity);

            _mockTokenService
                .Setup(t => t.GetUserId())
                .Returns(_mockUser.Object.Id);

            _mockUserManager
                .Setup(u => u.FindByIdAsync(_mockUser.Object.Id))
                .ReturnsAsync(_mockUser.Object);

            _mockAnswerVoteRepository
                .Setup(r => r.GetAsync(answerId, _mockUser.Object.Id))
                .ReturnsAsync(existingVote);

            _mockAnswerVoteRepository
                .Setup(r => r.DeleteAsync(answerId, _mockUser.Object.Id))
                .Returns(Task.CompletedTask);

            // Act
            await _voteService.CastVoteAsync(answerId, voteValue);

            // Assert
            _mockAnswerVoteRepository.Verify(
                r => r.DeleteAsync(answerId, _mockUser.Object.Id),
               Times.Once);
        }
    }
}
