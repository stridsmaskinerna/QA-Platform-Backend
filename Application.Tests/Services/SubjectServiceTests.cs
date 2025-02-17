using Application.Services;
using Domain.DTO.Response;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Moq;
using TestUtility.Factories;

namespace Application.Tests.Services;


// TODO!
// 1) Update SubjectService class methods to check if subject exist before
//    mapping to avoid returning null mapping or 200 OK when not found.
//    Better to return 404 not found.
// 2) If not exist call base method NotFound which throws an exception that will
//    result in a 404 not found response. 
// 3) If exist map and return.
public class SubjectServiceTests : SetupServiceTests
{
    SubjectService _subjectService;

    public SubjectServiceTests()
    {
        _subjectService = new SubjectService(
            _mockRepositoryManager.Object,
            _mockServiceManager.Object
        );
    }

    public class AddAsync : SubjectServiceTests
    {
        [Fact]
        public async Task ShouldReturnNewSubject_WhenSuccessful()
        {
            // Arrange
            var subjectId = Guid.NewGuid();

            var subjectRequestDto = SubjectFactory.CreateSubjectForCreationDTO(
               "Test Subject",
               "Test Subject Code"
            );

            var subjectEntity = SubjectFactory.CreateSubjectEntity(
                subjectId,
               "Test Subject",
               "Test Subject Code"
            );

            var emptyTeachers = new List<UserWithEmailDTO>();

            var createdSubjectDto = SubjectFactory.CreateSubjectDTO(
               subjectEntity.Id,
               subjectEntity.Name,
               subjectEntity.SubjectCode!,
               emptyTeachers
            );

            _mockMapper
                .Setup(m => m.Map<Subject>(subjectRequestDto))
                .Returns(subjectEntity);

            _mockSubjectRepository
                .Setup(r => r.AddAsync(subjectEntity))
                .ReturnsAsync(subjectEntity);

            _mockMapper
                .Setup(m => m.Map<SubjectDTO>(subjectEntity))
                .Returns(createdSubjectDto);

            // Act
            var result = await _subjectService.AddAsync(subjectRequestDto);

            // Assert
            // Assert
            Assert.NotNull(result);
            Assert.Equal(subjectEntity.Id, result.Id);
            Assert.Equal(subjectEntity.SubjectCode, result.SubjectCode);
            Assert.Equal(subjectEntity.Teachers.Count, result.Teachers.Count());

            _mockSubjectRepository.Verify(
                r => r.AddAsync(subjectEntity),
                Times.Once);

            _mockUserRepository.Verify(
                r => r.GetUserByMailAsync(It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task ShouldAddTeacher_WhenExistingTeacher()
        {
            // Arrange
            var subjectId = Guid.NewGuid();

            var teachers = new List<string>()
            {
                "testTeacherMail@ltu.se"
            };

            var teacherEntity = UserFactory.CreateUser(
                "testId", "testTeacher", teachers.First());

            var subjectDto = SubjectFactory.CreateSubjectForCreationDTO(
               "Test Subject",
               "Test Subject Code",
               teachers
            );

            var subjectEntity = SubjectFactory.CreateSubjectEntity(
                subjectId,
               "Test Subject",
               "Test Subject Code"
            );

            _mockMapper
                .Setup(m => m.Map<Subject>(subjectDto))
                .Returns(subjectEntity);

            _mockUserRepository
                .Setup(r => r.GetUserByMailAsync(subjectDto.Teachers.First()))
                .ReturnsAsync(teacherEntity);

            _mockSubjectRepository
                .Setup(r => r.AddAsync(subjectEntity))
                .ReturnsAsync(subjectEntity);

            _mockMapper
                .Setup(m => m.Map<SubjectDTO>(subjectEntity))
                .Returns(It.IsAny<SubjectDTO>());

            // Act
            await _subjectService.AddAsync(subjectDto);

            // Assert
            Assert.NotEmpty(subjectEntity.Teachers);
            Assert.Equal(teacherEntity, subjectEntity.Teachers.First());

            _mockUserRepository.Verify(
                r => r.GetUserByMailAsync(subjectDto.Teachers.First()),
                Times.Exactly(teachers.Count));
        }

        [Fact]
        public async Task ShouldNotAddTeacher_WhenNoExistingTeacher()
        {
            // Arrange
            var subjectId = Guid.NewGuid();

            var teachers = new List<string>()
            {
                "testTeacherMail@ltu.se"
            };

            var teacherEntity = UserFactory.CreateUser(
                "testId", "testTeacher", teachers.First());

            var subjectDto = SubjectFactory.CreateSubjectForCreationDTO(
               "Test Subject",
               "Test Subject Code",
               teachers
            );

            var subjectEntity = SubjectFactory.CreateSubjectEntity(
                subjectId,
               "Test Subject",
               "Test Subject Code"
            );

            _mockMapper
                .Setup(m => m.Map<Subject>(subjectDto))
                .Returns(subjectEntity);

            _mockUserRepository
                .Setup(r => r.GetUserByMailAsync(subjectDto.Teachers.First()))
                .ReturnsAsync(default(User)!);

            _mockSubjectRepository
                .Setup(r => r.AddAsync(subjectEntity))
                .ReturnsAsync(subjectEntity);

            _mockMapper
                .Setup(m => m.Map<SubjectDTO>(subjectEntity))
                .Returns(It.IsAny<SubjectDTO>());

            // Act
            await _subjectService.AddAsync(subjectDto);

            // Assert
            Assert.Empty(subjectEntity.Teachers);

            _mockUserRepository.Verify(
                r => r.GetUserByMailAsync(subjectDto.Teachers.First()),
                Times.Exactly(teachers.Count));
        }
    }

    public class DeleteAsync : SubjectServiceTests
    {
        [Fact]
        public async Task ShouldCallRepositoryDelete_WhenSubjectExist()
        {
            // Arrange
            var subjectId = Guid.NewGuid();

            var subject = SubjectFactory.CreateSubjectEntity(
                subjectId,
                "TestSubject",
                "TestCode");

            _mockSubjectRepository
                .Setup(r => r.GetByIdAsync(subjectId))
                .ReturnsAsync(subject);

            _mockSubjectRepository
                .Setup(r => r.DeleteAsync(subjectId))
                .Returns(Task.CompletedTask);

            // Act
            await _subjectService.DeleteAsync(subjectId);

            // Assert
            _mockSubjectRepository.Verify(
                r => r.DeleteAsync(subjectId),
                Times.Once);
        }

        [Fact]
        public async Task ShouldThrowNotFound_WhenSubjectDoesNotExist()
        {
            // Arrange
            var subjectId = Guid.NewGuid();

            _mockSubjectRepository
                .Setup(r => r.GetByIdAsync(subjectId))
                .ReturnsAsync(default(Subject));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => _subjectService.DeleteAsync(subjectId));
        }

        [Fact]
        public async Task ShouldNotCallRepositoryDelete_WhenSubjectDoesNotExist()
        {
            // Arrange
            var subjectId = Guid.NewGuid();

            _mockSubjectRepository
                .Setup(r => r.GetByIdAsync(subjectId))
                .ReturnsAsync(default(Subject));

            _mockSubjectRepository
                .Setup(r => r.DeleteAsync(subjectId))
                .Returns(Task.CompletedTask);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => _subjectService.DeleteAsync(subjectId));

            _mockSubjectRepository.Verify(
                r => r.DeleteAsync(subjectId),
                Times.Never);
        }
    }
}
