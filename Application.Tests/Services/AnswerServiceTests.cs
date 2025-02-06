using Application.Services;
using Application.Tests.Utilities;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;
using Domain.Exceptions;
using Moq;

namespace Application.Tests.Services;

public class AnswerServiceTests : BaseServiceSetupTests
{
    private readonly Mock<Question> _mockQuestion;
    private readonly Mock<User> _mockUser;
    private readonly AnswerService _answerService;
    private readonly Mock<ITokenService> _mockTokenService;

    public AnswerServiceTests()
    {
        _mockQuestion = new Mock<Question>();

        _mockUser = new Mock<User>();

        _mockTokenService = new Mock<ITokenService>();

        _answerService = new AnswerService(
            _mockRepositoryManager.Object,
            _mockServiceManager.Object);

        _mockServiceManager
            .Setup(sm => sm.TokenService)
            .Returns(_mockTokenService.Object);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnAnswerDTO_WhenSuccessful()
    {
        // Arrange
        var answerCreateDto = AnswerFactory.CreateAnswerForCreationDTO();

        var answerEntity = AnswerFactory.CreateAnswerEntity(
            Guid.NewGuid(), _mockQuestion.Object, _mockUser.Object);

        var userName = "TestUser";
        var answerDtoResponse = AnswerFactory.CreateAnswerDTO(
            answerEntity, userName);

        _mockMapper
            .Setup(m => m.Map<Answer>(answerCreateDto))
            .Returns(answerEntity);

        _mockTokenService
            .Setup(t => t.GetUserId())
            .Returns(_mockUser.Object.Id);

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
        var result = await _answerService.AddAsync(answerCreateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(answerEntity.Id, result.Id);
        Assert.Equal(userName, result.UserName);

        _mockAnswerRepository.Verify(
            r => r.AddAsync(It.IsAny<Answer>()),
            Times.Once);
    }

    [Fact]
    public async Task AddAsync_ShouldThrowBadRequest_WhenMappingFails()
    {
        // Arrange
        var answerCreateDto = AnswerFactory.CreateAnswerForCreationDTO();

        _mockMapper
            .Setup(m => m.Map<Answer>(answerCreateDto))
            .Returns((Answer)default!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
            _answerService.AddAsync(answerCreateDto));

        Assert.Equal(
            _answerService.MsgAddAsyncBadRequest(),
            exception.Message);
    }

    [Fact]
    public async Task AddAsync_ShouldNotCallAdd_WhenMappingFails()
    {
        // Arrange
        var answerCreateDto = AnswerFactory.CreateAnswerForCreationDTO();

        _mockMapper
            .Setup(m => m.Map<Answer>(answerCreateDto))
            .Returns((Answer)default!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
            _answerService.AddAsync(answerCreateDto));

        _mockAnswerRepository.Verify(
            r => r.AddAsync(It.IsAny<Answer>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateAnswer_WhenAnswerExists()
    {
        // Arrange
        var answerId = Guid.NewGuid();
        var answerEntity = AnswerFactory.CreateAnswerEntity(
           answerId, _mockQuestion.Object, _mockUser.Object);

        var answerPutDto = new AnswerForPutDTO
        {
            Value = "Updated Content"
        };

        _mockAnswerRepository
            .Setup(r => r.GetByIdAsync(answerId))
            .ReturnsAsync(answerEntity);

        _mockMapper
            .Setup(m => m.Map(answerPutDto, answerEntity));

        // Act
        await _answerService.UpdateAsync(answerId, answerPutDto);

        // Assert
        _mockMapper.Verify(
            m => m.Map(answerPutDto, answerEntity),
            Times.Once);

        _mockAnswerRepository.Verify(
            r => r.CompleteAsync(),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotFound_WhenAnswerDoesNotExist()
    {
        // Arrange
        var answerPutDto = AnswerFactory.CreateAnswerForPutDTO();

        var answerId = Guid.NewGuid();

        _mockAnswerRepository
            .Setup(r => r.GetByIdAsync(answerId))
            .ReturnsAsync((Answer)default!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            _answerService.UpdateAsync(answerId, answerPutDto));

        Assert.Equal(
            _answerService.MsgUpdateAsyncNotFound(answerId),
            exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_ShouldNotCallUpdate_WhenAnswerDoesNotExist()
    {
        // Arrange
        var answerPutDto = AnswerFactory.CreateAnswerForPutDTO();

        var answerId = Guid.NewGuid();

        _mockAnswerRepository
            .Setup(r => r.GetByIdAsync(answerId))
            .ReturnsAsync(default(Answer));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            _answerService.UpdateAsync(answerId, answerPutDto));

        _mockAnswerRepository.Verify(
            r => r.UpdateAsync(It.IsAny<Answer>()),
            Times.Never);
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
        _mockAnswerRepository.Verify(
            r => r.DeleteAsync(answerEntity),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowNotFound_WhenAnswerDoesNotExist()
    {
        // Arrange
        var answerId = Guid.NewGuid();

        _mockAnswerRepository
            .Setup(r => r.GetByIdAsync(answerId))
            .ReturnsAsync(default(Answer));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            _answerService.DeleteAsync(answerId));

        Assert.Equal(
            _answerService.MsgDeleteAsyncNotFound(answerId),
            exception.Message);
    }

    [Fact]
    public async Task DeleteAsync_ShouldNotCallDelete_WhenAnswerDoesNotExist()
    {
        // Arrange
        var answerId = Guid.NewGuid();

        _mockAnswerRepository
            .Setup(r => r.GetByIdAsync(answerId))
            .ReturnsAsync(default(Answer));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            _answerService.DeleteAsync(answerId));

        _mockAnswerRepository.Verify(
            r => r.UpdateAsync(It.IsAny<Answer>()),
            Times.Never);
    }
}
