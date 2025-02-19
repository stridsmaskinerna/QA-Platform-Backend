using Application.Contracts;
using Application.Services;
using Domain.DTO.Response;
using Domain.Entities;
using Domain.Exceptions;
using Moq;
using TestUtility.Factories;

namespace Application.Tests.Services;

public class QuestionServiceTests : SetupServiceTests
{
    private readonly QuestionService _questionService;
    private readonly Mock<Topic> _mockTopic;
    private readonly Mock<User> _mockUser;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<IDTOService> _mockDTOService;
    private readonly Mock<ITagService> _mockTagService;
    private readonly Mock<IUtilityService> _mockUtilityService;

    public QuestionServiceTests()
    {
        _mockUtilityService = new Mock<IUtilityService>();

        _mockTopic = new Mock<Topic>(MockBehavior.Strict);

        _mockUser = new Mock<User>();

        _mockTokenService = new Mock<ITokenService>(MockBehavior.Strict);

        _mockDTOService = new Mock<IDTOService>(MockBehavior.Strict);

        _mockTagService = new Mock<ITagService>(MockBehavior.Strict);

        _questionService = new QuestionService(
            _mockRepositoryManager.Object,
            _mockServiceManager.Object,
            _mockUserManager.Object);

        _mockServiceManager
            .Setup(sm => sm.TokenService)
            .Returns(_mockTokenService.Object);

        _mockServiceManager
            .Setup(sm => sm.DTOService)
            .Returns(_mockDTOService.Object);

        _mockServiceManager
            .Setup(sm => sm.TagService)
            .Returns(_mockTagService.Object);

        _mockServiceManager
            .Setup(sm => sm.UtilityService)
            .Returns(_mockUtilityService.Object);
    }

    public class GetByIdAsync : QuestionServiceTests
    {
        [Fact]
        public async Task ShouldReturnDetailedQuestion_WhenQuestionExists()
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
                .Setup(r => r.GetByIdAsync(questionId))
                .ReturnsAsync(questionEntity);

            _mockMapper
                .Setup(m => m.Map<QuestionDetailedDTO>(questionEntity))
                .Returns(questionDTO);

            _mockTokenService
                .Setup(s => s.GetUserId())
                .Returns(_mockUser.Object.Id);

            _mockDTOService
                .Setup(s => s.UpdatingAnsweredByTeacherField(questionDTO))
                .Returns(Task.CompletedTask);

            _mockDTOService
                .Setup(s => s.UpdatingMyVoteField(
                    questionEntity, questionDTO, _mockUser.Object.Id));

            _mockDTOService
                .Setup(s => s.UpdateQuestionIsHideableField(
                    questionDTO, _mockUser.Object.Id))
                .Returns(Task.CompletedTask);

            _mockDTOService
                .Setup(s => s.UpdateAnswerIsHideableField(questionDTO));

            // Act
            var result = await _questionService.GetByIdAsync(questionId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(questionEntity.Id, result.Id);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ShouldReturnDetailedQuestion_WhenQuestionDTOIsNotHidden(
            bool isHideable
        )
        {
            // Arrange
            var questionId = Guid.NewGuid();
            var subjectId = Guid.NewGuid();
            var answerId = Guid.NewGuid();

            var questionEntity = QuestionFactory.CreateQuestionEntity(
                questionId, _mockTopic.Object, _mockUser.Object);

            var questionDTO = QuestionFactory.CreateQuestionDetailedDto(
                questionId, _mockTopic.Object.Id);
            questionDTO.IsHidden = false;
            questionDTO.IsHideable = isHideable;

            _mockQuestionRepository
                .Setup(r => r.GetByIdAsync(questionId))
                .ReturnsAsync(questionEntity);

            _mockMapper
                .Setup(m => m.Map<QuestionDetailedDTO>(questionEntity))
                .Returns(questionDTO);

            _mockTokenService
                .Setup(s => s.GetUserId())
                .Returns(_mockUser.Object.Id);

            _mockDTOService
                .Setup(s => s.UpdatingAnsweredByTeacherField(questionDTO))
                .Returns(Task.CompletedTask);

            _mockDTOService
                .Setup(s => s.UpdatingMyVoteField(
                    questionEntity, questionDTO, _mockUser.Object.Id));

            _mockDTOService
                .Setup(s => s.UpdateQuestionIsHideableField(
                    questionDTO, _mockUser.Object.Id))
                .Returns(Task.CompletedTask);

            _mockDTOService
                .Setup(s => s.UpdateAnswerIsHideableField(questionDTO));

            // Act
            var result = await _questionService.GetByIdAsync(questionId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(questionEntity.Id, result.Id);
        }

        [Fact]
        public async Task ShouldAccurateCall_UpdatingDTOFieldMethods()
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
                .Setup(r => r.GetByIdAsync(questionId))
                .ReturnsAsync(questionEntity);

            _mockMapper
                .Setup(m => m.Map<QuestionDetailedDTO>(questionEntity))
                .Returns(questionDTO);

            _mockTokenService
                .Setup(s => s.GetUserId())
                .Returns(_mockUser.Object.Id);

            _mockDTOService
                .Setup(s => s.UpdatingAnsweredByTeacherField(questionDTO))
                .Returns(Task.CompletedTask);

            var sequence = new MockSequence();

            _mockDTOService.InSequence(sequence)
                .Setup(s => s.UpdatingMyVoteField(
                    questionEntity, questionDTO, _mockUser.Object.Id));

            _mockDTOService
                .Setup(s => s.UpdateAnswerIsHideableField(questionDTO));

            _mockDTOService.InSequence(sequence)
                .Setup(s => s.UpdateQuestionIsHideableField(
                    questionDTO, _mockUser.Object.Id))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _questionService.GetByIdAsync(questionId);

            // Assert
            _mockDTOService.Verify(
                s => s.UpdatingAnsweredByTeacherField(questionDTO),
                Times.Once);

            _mockDTOService.Verify(
                s => s.UpdatingMyVoteField(questionEntity, questionDTO, _mockUser.Object.Id),
                Times.Once);

            _mockDTOService.Verify(
                s => s.UpdateQuestionIsHideableField(questionDTO, _mockUser.Object.Id),
                Times.Once);

            _mockDTOService.Verify(
                s => s.UpdateAnswerIsHideableField(questionDTO),
                Times.Once);
        }

        [Fact]
        public async Task ShouldThrowNotFound_WhenQuestionIsNotFound()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            var answerId = Guid.NewGuid();


            _mockQuestionRepository
                .Setup(q => q.GetByIdAsync(questionId))
                .ReturnsAsync(default(Question));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => _questionService.GetByIdAsync(questionId));
        }

        [Fact]
        public async Task ShouldThrowNotFound_WhenQuestionDTOIsHiddenAndNotHideable()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            var answerId = Guid.NewGuid();

            var questionEntity = QuestionFactory.CreateQuestionEntity(
                questionId, _mockTopic.Object, _mockUser.Object);

            var questionDTO = QuestionFactory.CreateQuestionDetailedDto(
                questionId, _mockTopic.Object.Id);
            questionDTO.IsHideable = false;
            questionDTO.IsHidden = true;

            _mockQuestionRepository
                .Setup(r => r.GetByIdAsync(questionId))
                .ReturnsAsync(questionEntity);

            _mockMapper
                .Setup(m => m.Map<QuestionDetailedDTO>(questionEntity))
                .Returns(questionDTO);

            _mockTokenService
                .Setup(s => s.GetUserId())
                .Returns(_mockUser.Object.Id);

            _mockDTOService
                .Setup(s => s.UpdatingAnsweredByTeacherField(questionDTO))
                .Returns(Task.CompletedTask);

            var sequence = new MockSequence();

            _mockDTOService.InSequence(sequence)
                .Setup(s => s.UpdatingMyVoteField(
                    questionEntity, questionDTO, _mockUser.Object.Id));

            _mockDTOService
                .Setup(s => s.UpdateAnswerIsHideableField(questionDTO));

            _mockDTOService.InSequence(sequence)
                .Setup(s => s.UpdateQuestionIsHideableField(
                    questionDTO, _mockUser.Object.Id))
                .Returns(Task.CompletedTask);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => _questionService.GetByIdAsync(questionId));
        }

        [Fact]
        public async Task ShouldReturnDetailedQuestion_WhenQuestionDTOIsHiddenAndHideable()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            var answerId = Guid.NewGuid();

            var questionEntity = QuestionFactory.CreateQuestionEntity(
                questionId, _mockTopic.Object, _mockUser.Object);

            var questionDTO = QuestionFactory.CreateQuestionDetailedDto(
                questionId, _mockTopic.Object.Id);
            questionDTO.IsHideable = true;
            questionDTO.IsHidden = true;

            _mockQuestionRepository
                .Setup(r => r.GetByIdAsync(questionId))
                .ReturnsAsync(questionEntity);

            _mockMapper
                .Setup(m => m.Map<QuestionDetailedDTO>(questionEntity))
                .Returns(questionDTO);

            _mockTokenService
                .Setup(s => s.GetUserId())
                .Returns(_mockUser.Object.Id);

            _mockDTOService
                .Setup(s => s.UpdatingAnsweredByTeacherField(questionDTO))
                .Returns(Task.CompletedTask);

            var sequence = new MockSequence();

            _mockDTOService.InSequence(sequence)
                .Setup(s => s.UpdatingMyVoteField(
                    questionEntity, questionDTO, _mockUser.Object.Id));

            _mockDTOService
                .Setup(s => s.UpdateAnswerIsHideableField(questionDTO));

            _mockDTOService.InSequence(sequence)
                .Setup(s => s.UpdateQuestionIsHideableField(
                    questionDTO, _mockUser.Object.Id))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _questionService.GetByIdAsync(questionId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(questionEntity.Id, result.Id);
        }
    }

    public class GetPublicQuestionsByIdAsync : QuestionServiceTests
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

            _mockDTOService
                .Setup(t => t.UpdatingAnsweredByTeacherField(questionDTO))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _questionService.GetPublicQuestionByIdAsync(questionId);

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
                () => _questionService.GetPublicQuestionByIdAsync(questionId));

            Assert.Equal(
                QuestionService.MsgNotFound(questionId),
                exception.Message);
        }

        [Fact]
        public async Task ShouldAccurateCall_UpdatingDTOFieldMethods()
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

            _mockDTOService
                .Setup(t => t.UpdatingAnsweredByTeacherField(questionDTO))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _questionService.GetPublicQuestionByIdAsync(questionId);

            _mockDTOService.Verify(
                s => s.UpdatingAnsweredByTeacherField(questionDTO),
                Times.Once);
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
                .Setup(r => r.GetByIdAsync(questionId))
                .ReturnsAsync(question);

            _mockQuestionRepository
                .Setup(r => r.DeleteAsync(questionId))
                .Returns(Task.CompletedTask);

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

            Assert.Equal(QuestionService.MsgNotFound(questionId), exception.Message);
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

    public class AddAsync : QuestionServiceTests
    {
        [Fact]
        public async Task ShouldCreateQuestion_WhenValidData()
        {
            // Arrange
            var questionDTO = QuestionFactory.CreateQuestionForCreationDto(
                Guid.NewGuid(), Guid.NewGuid());

            var question = QuestionFactory.CreateQuestionEntity(
                Guid.NewGuid(), _mockTopic.Object, _mockUser.Object);

            var createdQuestionDTO = QuestionFactory.CreateQuestionDto(
                question.Id, _mockTopic.Object.Id);

            _mockMapper
                .Setup(m => m.Map<Question>(questionDTO))
                .Returns(question);

            _mockTopicRepository
                .Setup(r => r.GetByIdAsync(questionDTO.TopicId))
                .ReturnsAsync(_mockTopic.Object);

            _mockTokenService
                .Setup(t => t.GetUserId())
                .Returns(It.IsAny<string>());

            _mockTagService
                .Setup(t => t.StoreNewTagsFromQuestion(question, questionDTO.Tags))
                .Returns(Task.CompletedTask);

            _mockTokenService
                .Setup(t => t.GetUserName())
                .Returns(It.IsAny<string>());

            _mockUtilityService.Setup(
                r => r.NormalizeText(It.IsAny<string>()))
                .Returns(It.IsAny<string>());

            _mockQuestionRepository
                .Setup(r => r.AddAsync(question))
                .ReturnsAsync(question);

            _mockMapper
                .Setup(m => m.Map<QuestionDTO>(question))
                .Returns(createdQuestionDTO);

            // Act
            var result = await _questionService.AddAsync(questionDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(question.Id, result.Id);

            _mockTagService.Verify(
                r => r.StoreNewTagsFromQuestion(question, questionDTO.Tags),
                Times.Once);
        }

        [Fact]
        public async Task ShouldThrowBadRequest_WhenInvalidData()
        {
            // Arrange
            var questionDTO = QuestionFactory.CreateQuestionForCreationDto(
                Guid.NewGuid(), Guid.NewGuid());

            var question = QuestionFactory.CreateQuestionEntity(
                Guid.NewGuid(), _mockTopic.Object, _mockUser.Object);

            _mockMapper
                .Setup(m => m.Map<Question>(questionDTO))
                .Returns(question);

            _mockTopicRepository
                .Setup(r => r.GetByIdAsync(questionDTO.TopicId))
                .ReturnsAsync(default(Topic));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
                _questionService.AddAsync(questionDTO));

            Assert.Equal(
                QuestionService.MsgBadRequest(),
                exception.Message);
        }

        [Fact]
        public async Task ShouldNotCallAdd_WhenInvalidData()
        {
            // Arrange
            var questionDTO = QuestionFactory.CreateQuestionForCreationDto(
                Guid.NewGuid(), Guid.NewGuid());

            var question = QuestionFactory.CreateQuestionEntity(
                Guid.NewGuid(), _mockTopic.Object, _mockUser.Object);

            _mockMapper
                .Setup(m => m.Map<Question>(questionDTO))
                .Returns(default(Question)!);

            _mockTopicRepository
                .Setup(r => r.GetByIdAsync(questionDTO.TopicId))
                .ReturnsAsync(_mockTopic.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
                _questionService.AddAsync(questionDTO));

            _mockQuestionRepository.Verify(
                r => r.AddAsync(question),
                Times.Never);

            _mockTagRepository.Verify(
                r => r.AddAsync(It.IsAny<Tag>()),
                Times.Never);
        }
    }

    public class UpdateAsync : QuestionServiceTests
    {
        [Fact]
        public async Task ShouldUpdateQuestion_WhenQuestionExist()
        {
            // Arrange
            var questionId = Guid.NewGuid();

            var questionDTO = QuestionFactory.CreateQuestionForPutDto(
                questionId, Guid.NewGuid());

            var question = QuestionFactory.CreateQuestionEntity(
                questionId, _mockTopic.Object, _mockUser.Object);

            _mockQuestionRepository
                .Setup(r => r.GetByIdAsync(questionId))
                .ReturnsAsync(question);

            _mockMapper
                .Setup(m => m.Map(questionDTO, question))
                .Returns(question);

            _mockUtilityService.Setup(r => r.NormalizeText(It.IsAny<string>()))
                .Returns(It.IsAny<string>()); ;

            _mockQuestionRepository
                .Setup(r => r.CompleteAsync())
                .Returns(Task.CompletedTask);

            _mockTagRepository
                .Setup(r => r.DeleteUnusedTagsAsync(It.IsAny<IEnumerable<Tag>>()))
                .Returns(Task.CompletedTask);

            _mockTagService
                .Setup(s => s.StoreNewTagsFromQuestion(question, questionDTO.Tags))
                .Returns(Task.CompletedTask);

            _mockQuestionRepository
                .Setup(r => r.UpdateAsync(question))
                .Returns(Task.CompletedTask);

            // Act
            await _questionService.UpdateAsync(
                questionId, questionDTO);

            // Assert
            _mockQuestionRepository.Verify(
                r => r.CompleteAsync(),
                Times.Once);

            _mockTagRepository.Verify(
                r => r.DeleteUnusedTagsAsync(It.IsAny<IEnumerable<Tag>>()),
                Times.Once);

            _mockTagService.Verify(
                r => r.StoreNewTagsFromQuestion(question, questionDTO.Tags),
                Times.Once);
        }

        [Fact]
        public async Task ShouldThrowNotFound_WhenQuestionDoesNotExist()
        {
            // Arrange
            var questionId = Guid.NewGuid();

            var questionDTO = QuestionFactory.CreateQuestionForPutDto(
               Guid.NewGuid(), Guid.NewGuid());

            _mockQuestionRepository
                .Setup(r => r.GetByIdAsync(questionId))
                .ReturnsAsync(default(Question));

            _mockMapper
                .Setup(m => m.Map(questionDTO, It.IsAny<Question>()))
                .Returns(It.IsAny<Question>());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
                _questionService.UpdateAsync(questionId, questionDTO));

            Assert.Equal(
                QuestionService.MsgNotFound(questionId),
                exception.Message);
        }

        [Fact]
        public async Task ShouldNotCallUpdate_WhenQuestionDoesNotExist()
        {
            // Arrange
            var questionId = Guid.NewGuid();

            var questionDTO = QuestionFactory.CreateQuestionForPutDto(
               Guid.NewGuid(), Guid.NewGuid());

            _mockQuestionRepository
                .Setup(r => r.GetByIdAsync(questionId))
                .ReturnsAsync(default(Question));

            _mockMapper
                .Setup(m => m.Map(questionDTO, It.IsAny<Question>()))
                .Returns(It.IsAny<Question>());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
                _questionService.UpdateAsync(questionId, questionDTO));

            _mockQuestionRepository.Verify(
                r => r.CompleteAsync(),
                Times.Never);

            _mockTagRepository.Verify(
                r => r.DeleteUnusedTagsAsync(It.IsAny<IEnumerable<Tag>>()),
                Times.Never);
        }
    }
}
