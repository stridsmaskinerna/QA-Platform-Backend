using Application.Services;
using Application.Tests.Utilities;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;
using Domain.Exceptions;
using Moq;


namespace Application.Tests.Services;

public class AnswerServiceTests : BaseServiceTest
{
    private readonly Mock<Question> _mockQuestion;
    private readonly Mock<User> _mockUser;
    private readonly AnswerService _answerService;

    public AnswerServiceTests()
    {
        _mockQuestion = new Mock<Question>();
        _mockUser = new Mock<User>();

        _answerService = new AnswerService(
            _mockRepositoryManager.Object,
            _mockServiceManager.Object);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnAnswerDTO_WhenSuccessful()
    {
        // Arrange
        var answerDtoRequest = AnswerFactory.CreateAnswerForCreationDTO();

        var answerEntity = AnswerFactory.CreateAnswerEntity(
            Guid.NewGuid(), _mockQuestion.Object, _mockUser.Object);

        var userName = "TestUser";
        var answerDtoResponse = AnswerFactory.CreateAnswerDTO(
            answerEntity, userName);

        _mockMapper
            .Setup(m => m.Map<Answer>(answerDtoRequest))
            .Returns(answerEntity);

        _mockTokenService
            .Setup(t => t.GetUserId())
            .Returns("UserTestId");

        _mockAnswerRepository
            .Setup(r => r.AddAsync(It.IsAny<Answer>()))
            .ReturnsAsync(answerEntity);

        _mockMapper
            .Setup(m => m.Map<AnswerDTO>(answerEntity))
            .Returns(answerDtoResponse);

        _mockTokenService
            .Setup(t => t.GetUserName())
            .Returns(userName);

        // Act
        var result = await _answerService.AddAsync(answerDtoRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(answerEntity.Id, result.Id);
        Assert.Equal(userName, result.UserName);

        _mockAnswerRepository.Verify(r => r.AddAsync(It.IsAny<Answer>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_ShouldThrowBadRequest_WhenMappingFails()
    {
        // Arrange
        var answerDtoRequest = AnswerFactory.CreateAnswerForCreationDTO();

        _mockMapper
            .Setup(m => m.Map<Answer>(It.IsAny<AnswerForCreationDTO>()))
            .Returns((Answer)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
            _answerService.AddAsync(answerDtoRequest));

        Assert.Equal(_answerService.MsgAddAsyncBadRequest(), exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateAnswer_WhenAnswerExists()
    {
        // Arrange
        var answerId = Guid.NewGuid();
        var answerEntity = AnswerFactory.CreateAnswerEntity(
           answerId, _mockQuestion.Object, _mockUser.Object);

        var updatedDto = new AnswerForPutDTO
        {
            Value = "Updated Content"
        };

        _mockAnswerRepository
            .Setup(r => r.GetByIdAsync(answerId))
            .ReturnsAsync(answerEntity);

        _mockMapper
            .Setup(m => m.Map(updatedDto, answerEntity));

        // Act
        await _answerService.UpdateAsync(answerId, updatedDto);

        // Assert
        _mockMapper.Verify(m => m.Map(updatedDto, answerEntity), Times.Once);
        _mockAnswerRepository.Verify(r => r.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotFound_WhenAnswerDoesNotExist()
    {
        // Arrange
        var updatedDto = AnswerFactory.CreateAnswerForPutDTO();

        var answerId = Guid.NewGuid();

        _mockAnswerRepository
            .Setup(r => r.GetByIdAsync(answerId))
            .ReturnsAsync((Answer)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            _answerService.UpdateAsync(answerId, updatedDto));

        Assert.Equal(_answerService.MsgUpdateAsyncNotFound(answerId), exception.Message);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteAnswer_WhenAnswerExists()
    {
        // Arrange
        var answerId = Guid.NewGuid();
        var answerEntity = AnswerFactory.CreateAnswerEntity(
            answerId, _mockQuestion.Object, _mockUser.Object);

        _mockAnswerRepository
            .Setup(r => r.GetByIdAsync(answerId))
            .ReturnsAsync(answerEntity);

        _mockAnswerRepository
            .Setup(r => r.DeleteAsync(answerEntity));

        // Act
        await _answerService.DeleteAsync(answerId);

        // Assert
        _mockAnswerRepository.Verify(r => r.DeleteAsync(answerEntity), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowNotFound_WhenAnswerDoesNotExist()
    {
        // Arrange
        var answerId = Guid.NewGuid();

        _mockAnswerRepository
            .Setup(r => r.GetByIdAsync(answerId))
            .ReturnsAsync((Answer)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            _answerService.DeleteAsync(answerId));

        Assert.Equal(_answerService.MsgDeleteAsyncNotFound(answerId), exception.Message);
    }
}
