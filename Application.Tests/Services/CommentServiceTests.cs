using Application.Services;
using Application.Tests.Utilities;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;
using Domain.Exceptions;
using Moq;


namespace Application.Tests.Services;

public class CommentServiceTests : BaseServiceTest
{
    private readonly Mock<User> _mockUser;
    private readonly Mock<Answer> _mockAnswer;
    private readonly CommentService _commentService;

    public CommentServiceTests()
    {
        _mockUser = new Mock<User>();
        _mockAnswer = new Mock<Answer>();

        _commentService = new CommentService(
            _mockRepositoryManager.Object,
            _mockServiceManager.Object);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnCommentDTO_WhenSuccessful()
    {
        // Arrange
        var answerId = Guid.NewGuid();
        var commentDtoRequest = CommentFactory.CreateCommentForCreationDTO(answerId);

        var commentId = Guid.NewGuid();
        var commentEntity = CommentFactory.CreateCommentEntity(
            commentId, _mockAnswer.Object, _mockUser.Object);

        var userName = "TestUser";
        var commentDtoResponse = CommentFactory.CreateCommentDTO(
            commentEntity, userName);

        _mockMapper
            .Setup(m => m.Map<Comment>(commentDtoRequest))
            .Returns(commentEntity);

        _mockTokenService
            .Setup(t => t.GetUserId())
            .Returns("UserTestId");

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
        var result = await _commentService.AddAsync(commentDtoRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(commentEntity.Id, result.Id);
        Assert.Equal(userName, result.UserName);

        _mockCommentRepository.Verify(r => r.AddAsync(It.IsAny<Comment>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_ShouldThrowBadRequest_WhenMappingFails()
    {
        // Arrange
        var answerId = Guid.NewGuid();
        var commentDtoRequest = CommentFactory.CreateCommentForCreationDTO(answerId);


        _mockMapper
            .Setup(m => m.Map<Comment>(It.IsAny<CommentForCreationDTO>()))
            .Returns((Comment)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
            _commentService.AddAsync(commentDtoRequest));

        Assert.Equal(_commentService.MsgAddAsyncBadRequest(), exception.Message);
    }
}
