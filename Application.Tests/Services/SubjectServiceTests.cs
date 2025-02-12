using Application.Services;
using Domain.DTO.Response;
using Domain.Entities;
using Moq;
using TestUtility.Factories;

namespace Application.Tests.Services;


// TODO!
// 1) Update SubjectService class methods to check if subject exist before mapping.
// 2) If not exist call base method not found
// 3) If exist map and return.
// 4) No try and catch should be used in service classes; exception handled by global exception middleware.
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
}
