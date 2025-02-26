using Application.Contracts;
using Application.Services;
using Domain.DTO.Query;
using Domain.DTO.Response;
using Domain.Entities;
using Domain.Exceptions;
using Moq;
using TestUtility.Factories;

namespace Application.Tests.Services;

public class TagServiceTests : SetupServiceTests
{
    private readonly TagService _tagService;
    private readonly Mock<IUtilityService> _mockUtilityService;
    private readonly Mock<Topic> _mockTopic;
    private readonly Mock<User> _mockUser;

    public TagServiceTests()
    {
        _tagService = new TagService(
            _mockRepositoryManager.Object,
            _mockServiceManager.Object);

        _mockUtilityService = new Mock<IUtilityService>(MockBehavior.Strict);
        _mockServiceManager.Setup(s => s.UtilityService).Returns(_mockUtilityService.Object);

        _mockTopic = new Mock<Topic>();
        _mockUser = new Mock<User>();
    }

    public class AddAsync : TagServiceTests
    {
        [Fact]
        public async Task ShouldReturnTagDTO_WhenTagIsAdded()
        {
            // Arrange
            var tagEntity = TagFactory.CreateTag(Guid.NewGuid(), "TestTag");

            var tagDTO = TagFactory.CreateTagStandardDTO(tagEntity.Id, tagEntity.Value);

            _mockTagRepository
                .Setup(r => r.AddAsync(tagEntity))
                .ReturnsAsync(tagEntity);

            _mockMapper
                .Setup(m => m.Map<TagStandardDTO>(tagEntity))
                .Returns(tagDTO);

            // Act
            var result = await _tagService.AddAsync(tagEntity);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(tagDTO.Id, result.Id);
            Assert.Equal(tagDTO.Value, result.Value);
        }
    }

    public class DeleteAsync : TagServiceTests
    {
        [Fact]
        public async Task ShouldThrowNotFound_WhenTagDoesNotExist()
        {
            // Arrange
            var tagId = Guid.NewGuid();
            _mockTagRepository
                .Setup(s => s.GetByIdAsync(tagId))
                .ReturnsAsync(default(Tag));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => _tagService.DeleteAsync(tagId));

            Assert.Equal(TagService.MsgNotFound(), exception.Message);
        }

        [Fact]
        public async Task ShouldCallRepository_WhenTagExists()
        {
            // Arrange
            var tagId = Guid.NewGuid();

            var tagEntity = TagFactory.CreateTag(tagId, "TestTag");

            _mockTagRepository
                .Setup(s => s.GetByIdAsync(tagId))
                .ReturnsAsync(tagEntity);

            _mockTagRepository
                .Setup(r => r.DeleteAsync(tagEntity.Id))
                .Returns(Task.CompletedTask);

            // Act
            await _tagService.DeleteAsync(tagId);

            // Assert
            _mockTagRepository.Verify(
                r => r.DeleteAsync(tagId),
                Times.Once);
        }
    }

    // TODO! Update test after refactoring
    //public class GetAllAsync : TagServiceTests
    //{
    //    [Fact]
    //    public async Task ShouldReturnListOfTags()
    //    {
    //        // Arrange
    //        var tagEntities = new List<Tag>
    //        {
    //            TagFactory.CreateTag(Guid.NewGuid(), "TestTag")
    //        };

    //        var paginatDTO = new PaginationDTO();

    //        var tagDTOs = new List<TagStandardDTO> {
    //            TagFactory.CreateTagStandardDTO(tagEntities[0].Id, tagEntities[0].Value)
    //        };

    //        _mockTagRepository
    //            .Setup(r => r.GetAllAsync())
    //            .ReturnsAsync(tagEntities);

    //        _mockMapper
    //            .Setup(m => m.Map<IEnumerable<TagStandardDTO>>(tagEntities))
    //            .Returns(tagDTOs);

    //        // Act
    //        var result = await _tagService.GetAllAsync();

    //        // Assert
    //        Assert.NotNull(result);
    //        Assert.Single(result);
    //    }
    //}

    public class GetFilteredList : TagServiceTests
    {
        [Fact]
        public async Task ShouldReturnFilteredTags()
        {
            // Arrange
            var value = "Filter";
            var tagEntities = new List<Tag>
            {
                TagFactory.CreateTag(Guid.NewGuid(), value)
            };

            var tagDTOs = new List<TagStandardDTO> {
                TagFactory.CreateTagStandardDTO(tagEntities[0].Id, tagEntities[0].Value)
            };

            _mockTagRepository
                .Setup(r => r.GetFilteredList(value))
                .ReturnsAsync(tagEntities);

            _mockMapper
                .Setup(m => m.Map<IEnumerable<TagStandardDTO>>(tagEntities))
                .Returns(tagDTOs);

            // Act
            var result = await _tagService.GetFilteredList(value);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }
    }

    public class GetByIdAsync : TagServiceTests
    {
        [Fact]
        public async Task ShouldReturnTag_WhenTagExists()
        {
            // Arrange
            var tagId = Guid.NewGuid();

            var tagEntity = TagFactory.CreateTag(Guid.NewGuid(), "TestTag");

            var tagDTO = TagFactory.CreateTagStandardDTO(tagEntity.Id, tagEntity.Value);

            _mockTagRepository
                .Setup(s => s.GetByIdAsync(tagId))
                .ReturnsAsync(tagEntity);

            _mockMapper
                .Setup(m => m.Map<TagStandardDTO>(tagEntity))
                .Returns(tagDTO);

            // Act
            var result = await _tagService.GetByIdAsync(tagId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(tagDTO.Id, result.Id);
        }

        [Fact]
        public async Task ShouldThrowNotFound_WhenTagDoesNotExist()
        {
            // Arrange
            var tagId = Guid.NewGuid();

            _mockTagRepository
                .Setup(s => s.GetByIdAsync(tagId))
                .ReturnsAsync(default(Tag));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => _tagService.GetByIdAsync(tagId));

            Assert.Equal(TagService.MsgNotFound(), exception.Message);
        }
    }

    public class GetByValueAsync : TagServiceTests
    {
        [Fact]
        public async Task ShouldReturnTag_WhenTagExists()
        {
            // Arrange
            var value = "TestTag";

            var tagEntity = TagFactory.CreateTag(Guid.NewGuid(), value);

            var tagDTO = TagFactory.CreateTagStandardDTO(tagEntity.Id, tagEntity.Value);

            _mockTagRepository
                .Setup(r => r.GetByValueAsync(value))
                .ReturnsAsync(tagEntity);

            _mockMapper
                .Setup(m => m.Map<TagStandardDTO>(tagEntity))
                .Returns(tagDTO);

            // Act
            var result = await _tagService.GetByValueAsync(value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(value, result.Value);
        }
    }

    public class IsTagValueTakenAsync : TagServiceTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ShouldReturnTrue_WhenTagExists(
            bool expected
        )
        {
            // Arrange
            var tagValue = "TestTag";
            _mockTagRepository
                .Setup(r => r.IsTagValueTakenAsync(tagValue))
                .ReturnsAsync(expected);

            // Act
            var result = await _tagService.IsTagValueTakenAsync(tagValue);

            // Assert
            Assert.Equal(expected, result);
        }
    }

    public class UpdateAsync : TagServiceTests
    {
        [Fact]
        public async Task ShouldCallRepository()
        {
            // Arrange
            var tagEntity = TagFactory.CreateTag(Guid.NewGuid(), "TestTag");

            _mockTagRepository
                .Setup(r => r.UpdateAsync(tagEntity))
                .Returns(Task.CompletedTask);

            // Act
            await _tagService.UpdateAsync(tagEntity);

            // Assert
            _mockTagRepository.Verify(
                r => r.UpdateAsync(tagEntity),
                Times.Once);
        }
    }

    public class StoreNewTagsFromQuestion : TagServiceTests
    {
        [Fact]
        public async Task ShouldNotCallRepository_WhenNormalizedTagsDoesExist()
        {
            // Arrange
            var tagValue = "testTag";

            var tags = new List<string> { tagValue };

            var storedTag = TagFactory.CreateTag(Guid.NewGuid(), tagValue);

            var question = QuestionFactory.CreateQuestionEntity(
                Guid.NewGuid(), _mockTopic.Object, _mockUser.Object);

            _mockUtilityService
                .Setup(s => s.NormalizeText(tagValue))
                .Returns(tagValue);

            _mockTagRepository
                .Setup(r => r.GetByValueAsync(tags[0]))
                .ReturnsAsync(storedTag);

            // Act
            await _tagService.StoreNewTagsFromQuestion(question, tags);

            // Assert
            _mockTagRepository.Verify(
                r => r.AddAsync(storedTag),
                Times.Never);
        }

        [Fact]
        public async Task ShouldCallRepository_WhenNormalizedTagsDoesNotExist()
        {
            // Arrange
            var tagValue = "testTag";

            var tags = new List<string> { tagValue };


            var storedTag = TagFactory.CreateTag(Guid.NewGuid(), tagValue);

            var newTag = TagFactory.CreateTag(tagValue);

            var question = QuestionFactory.CreateQuestionEntity(
                Guid.NewGuid(), _mockTopic.Object, _mockUser.Object);

            _mockUtilityService
                .Setup(s => s.NormalizeText(tagValue))
                .Returns(tagValue);

            _mockTagRepository
                .Setup(r => r.GetByValueAsync(tags[0]))
                .ReturnsAsync(default(Tag));

            _mockTagRepository
                .Setup(r => r.AddAsync(It.IsAny<Tag>()))
                .ReturnsAsync(It.IsAny<Tag>());

            // Act
            await _tagService.StoreNewTagsFromQuestion(question, tags);

            // Assert
            _mockTagRepository.Verify(
                r => r.AddAsync(It.IsAny<Tag>()),
                Times.Once);
        }
    }
}
