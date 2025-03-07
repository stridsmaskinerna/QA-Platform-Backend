using Application.Services;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;
using Domain.Exceptions;
using Moq;
using TestUtility.Factories;

namespace Application.Tests.Services;

public class CommentServiceTests : SetupServiceTests
{
    private readonly CommentService _commentService;
    private readonly Mock<User> _mockUser;
    private readonly Mock<Answer> _mockAnswer;
    private readonly Mock<ITokenService> _mockTokenService;

    public CommentServiceTests()
    {
        _mockUser = new Mock<User>();

        _mockAnswer = new Mock<Answer>();

        _mockTokenService = new Mock<ITokenService>();

        _commentService = new CommentService(
            _mockRepositoryManager.Object,
            _mockServiceManager.Object);

        _mockServiceManager
            .Setup(sm => sm.TokenService)
            .Returns(_mockTokenService.Object);
    }

    public class AddAsync : CommentServiceTests
    {
        [Fact]
        public async Task ShouldReturnCommentDTO_WhenSuccessful()
        {
            // Arrange
            var answerId = Guid.NewGuid();
            var commentCreateDto = CommentFactory.CreateCommentForCreationDTO(answerId);

            var commentId = Guid.NewGuid();
            var commentEntity = CommentFactory.CreateCommentEntity(
                commentId, _mockAnswer.Object, _mockUser.Object);

            var userName = "TestUser";
            var commentDtoResponse = CommentFactory.CreateCommentDTO(
                commentEntity, userName);

            _mockMapper
                .Setup(m => m.Map<Comment>(commentCreateDto))
                .Returns(commentEntity);

            _mockTokenService
                .Setup(t => t.GetUserId())
                .Returns(_mockUser.Object.Id);

            _mockCommentRepository
                .Setup(r => r.AddAsync(It.IsAny<Comment>()))
                .ReturnsAsync(commentEntity);

            _mockMapper
                .Setup(m => m.Map<CommentDTO>(commentEntity))
                .Returns(commentDtoResponse);

            _mockTokenService
                .Setup(t => t.GetUserName())
                .Returns(userName);

            // Act
            var result = await _commentService.AddAsync(commentCreateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(commentEntity.Id, result.Id);
            Assert.Equal(userName, result.UserName);

            _mockCommentRepository.Verify(r => r.AddAsync(commentEntity), Times.Once);
        }

        [Fact]
        public async Task ShouldThrowBadRequest_WhenMappingFails()
        {
            // Arrange
            var commentCreateDto = CommentFactory.CreateCommentForCreationDTO(
                Guid.NewGuid());

            _mockMapper
                .Setup(m => m.Map<Comment>(It.IsAny<CommentForCreationDTO>()))
                .Returns((Comment)default!);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
                _commentService.AddAsync(commentCreateDto));

            Assert.Equal(
                CommentService.MsgAddAsyncBadRequest(),
                exception.Message);
        }

        [Fact]
        public async Task ShouldNotCallAdd_WhenMappingFails()
        {
            // Arrange
            var commentCreateDto = CommentFactory.CreateCommentForCreationDTO(
                Guid.NewGuid());

            _mockMapper
                .Setup(m => m.Map<Comment>(It.IsAny<CommentForCreationDTO>()))
                .Returns((Comment)default!);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
                _commentService.AddAsync(commentCreateDto));

            _mockCommentRepository.Verify
                (r => r.AddAsync(It.IsAny<Comment>()),
                Times.Never);
        }

    }

    public class UpdateAsync : CommentServiceTests
    {
        [Fact]
        public async Task ShouldUpdateComment_WhenCommentExist()
        {
            // Arrange
            var commentId = Guid.NewGuid();

            var commentEntity = CommentFactory.CreateCommentEntity(
                commentId, _mockAnswer.Object, _mockUser.Object);

            var commentPutDto = CommentFactory.CreateCommentForPutDTO();

            _mockCommentRepository
                .Setup(r => r.GetByIdAsync(commentId))
                .ReturnsAsync(commentEntity);

            _mockMapper
                .Setup(m => m.Map(commentPutDto, commentEntity))
                .Returns(It.IsAny<Comment>());

            _mockCommentRepository
                .Setup(r => r.UpdateAsync(It.IsAny<Comment>()))
                .Returns(Task.CompletedTask);

            // Act
            await _commentService.UpdateAsync(commentId, commentPutDto);

            // Assert
            _mockMapper.Verify(
                m => m.Map(commentPutDto, commentEntity),
                Times.Once);

            _mockCommentRepository.Verify(
                r => r.UpdateAsync(It.IsAny<Comment>()),
                Times.Once);
        }

        [Fact]
        public async Task ShouldThrowNotFound_WhenCommentDoesNotExist()
        {
            // Arrange
            var commentPutDto = CommentFactory.CreateCommentForPutDTO();

            var commentId = Guid.NewGuid();

            _mockCommentRepository
                .Setup(r => r.GetByIdAsync(commentId))
                .ReturnsAsync(default(Comment));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
                _commentService.UpdateAsync(commentId, commentPutDto));

            Assert.Equal(
                CommentService.MsgNotFound(commentId),
                exception.Message);
        }

        [Fact]
        public async Task ShouldNotCallUpdate_WhenCommentDoesNotExist()
        {
            // Arrange
            var commentPutDto = CommentFactory.CreateCommentForPutDTO();

            var commentId = Guid.NewGuid();

            _mockCommentRepository
                .Setup(r => r.GetByIdAsync(commentId))
                .ReturnsAsync(default(Comment));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
                _commentService.UpdateAsync(commentId, commentPutDto));

            _mockCommentRepository.Verify(
                r => r.UpdateAsync(It.IsAny<Comment>()),
                Times.Never);
        }
    }

    public class DeleteAsync : CommentServiceTests
    {
        [Fact]
        public async Task ShouldDeleteComment_WhenCommentExists()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var commentEntity = CommentFactory.CreateCommentEntity(
                commentId, _mockAnswer.Object, _mockUser.Object);

            _mockCommentRepository
                .Setup(r => r.GetByIdAsync(commentId))
                .ReturnsAsync(commentEntity);

            _mockCommentRepository
                .Setup(r => r.DeleteAsync(commentEntity))
                .Returns(Task.CompletedTask);

            // Act
            await _commentService.DeleteAsync(commentId);

            // Assert
            _mockCommentRepository.Verify(
                r => r.DeleteAsync(commentEntity),
                Times.Once);
        }

        [Fact]
        public async Task ShouldThrowNotFound_WhenCommentDoesNotExists()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var commentEntity = CommentFactory.CreateCommentEntity(
                commentId, _mockAnswer.Object, _mockUser.Object);

            _mockCommentRepository
                .Setup(r => r.GetByIdAsync(commentId))
                .ReturnsAsync(default(Comment));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
                _commentService.DeleteAsync(commentId));

            Assert.Equal(
                CommentService.MsgNotFound(commentId),
                exception.Message);
        }

        [Fact]
        public async Task ShouldNotCallDelete_WhenCommentDoesNotExists()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var commentEntity = CommentFactory.CreateCommentEntity(
                commentId, _mockAnswer.Object, _mockUser.Object);

            _mockCommentRepository
                .Setup(r => r.GetByIdAsync(commentId))
                .ReturnsAsync(default(Comment));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
                _commentService.DeleteAsync(commentId));

            _mockCommentRepository.Verify(
                r => r.DeleteAsync(commentEntity),
                Times.Never);
        }
    }
}
