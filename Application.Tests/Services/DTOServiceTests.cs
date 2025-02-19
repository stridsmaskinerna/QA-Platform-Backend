using Application.Services;
using Domain.DTO.Response;
using Domain.Entities;
using Moq;
using TestUtility.Factories;

namespace Application.Tests.Services;

public class DTOServiceTests : SetupServiceTests
{
    private readonly DTOService _dtoService;

    public DTOServiceTests()
    {
        _dtoService = new DTOService(_mockRepositoryManager.Object);
    }

    public class UpdateQuestionIsHideableField : DTOServiceTests
    {
        [Fact]
        public async Task UpdateQuestionIsHideableField_ShouldSetIsHideableCorrectlyForListOfDTO()
        {
            // Arrange
            string userId = "teacher123";
            var subjectId1 = Guid.NewGuid();
            var subjectId2 = Guid.NewGuid();

            var dtoList = new List<QuestionDTO>
            {
                QuestionFactory.CreateQuestionDto(Guid.NewGuid(), Guid.NewGuid(), subjectId1),
                QuestionFactory.CreateQuestionDto(Guid.NewGuid(), Guid.NewGuid(), subjectId2)
            };

            var teachersSubjects = new List<Subject> { SubjectFactory.CreateSubjectEntity(
                subjectId1, "testName", "testCode") };

            _mockSubjectRepository
                .Setup(repo => repo.GetTeachersSubjectsAsync(userId))
                .ReturnsAsync(teachersSubjects);

            // Act
            await _dtoService.UpdateQuestionIsHideableField(dtoList, userId);

            // Assert
            Assert.True(dtoList[0].IsHideable);
            Assert.False(dtoList[1].IsHideable);
        }

        [Fact]
        public async Task UpdateQuestionIsHideableField_ShouldSetIsHideableCorrectlyForSingleDTO()
        {
            // Arrange
            string userId = "testTeacher";
            var subjectId = Guid.NewGuid();

            var questionDTO = QuestionFactory.CreateQuestionDto(Guid.NewGuid(), Guid.NewGuid(), subjectId);

            var teachersSubjects = new List<Subject> { SubjectFactory.CreateSubjectEntity(
            subjectId, "testName", "testCode") };

            _mockSubjectRepository
                .Setup(repo => repo.GetTeachersSubjectsAsync(userId))
                .ReturnsAsync(teachersSubjects);

            // Act
            await _dtoService.UpdateQuestionIsHideableField(questionDTO, userId);

            // Assert
            Assert.True(questionDTO.IsHideable);
        }
    }

    public class UpdateAnswerIsHideableField : DTOServiceTests
    {
        [Fact]
        public void ShouldUpdateAnswers_WhenQuestionIsHideable()
        {
            // Arrange
            var questionDTO = QuestionFactory.CreateQuestionDetailedDto(
                    Guid.NewGuid(), Guid.NewGuid());

            var answerDTOs = new List<AnswerDetailedDTO> {
            AnswerFactory.CreateAnswerDetailedDTO(Guid.NewGuid(), "value", "userName1", false),
            AnswerFactory.CreateAnswerDetailedDTO(Guid.NewGuid(), "value", "userName2", false)
        };

            questionDTO.IsHideable = true;

            questionDTO.Answers = answerDTOs;

            // Act
            _dtoService.UpdateAnswerIsHideableField(questionDTO);

            // Assert
            Assert.All(
                questionDTO.Answers,
                answer => Assert.True(answer.IsHideable));
        }

        [Fact]
        public void ShouldNotUpdateAnswers_WhenQuestionIsNotHideable()
        {
            // Arrange
            var questionDTO = QuestionFactory.CreateQuestionDetailedDto(
                    Guid.NewGuid(), Guid.NewGuid());

            var answerDTOs = new List<AnswerDetailedDTO> {
            AnswerFactory.CreateAnswerDetailedDTO(Guid.NewGuid(), "value", "userName1", false),
            AnswerFactory.CreateAnswerDetailedDTO(Guid.NewGuid(), "value", "userName2", false)
        };

            questionDTO.IsHideable = false;

            questionDTO.Answers = answerDTOs;

            // Act
            _dtoService.UpdateAnswerIsHideableField(questionDTO);

            // Assert
            Assert.All(
                questionDTO.Answers,
                answer => Assert.False(answer.IsHideable));
        }
    }

    public class UpdatingAnsweredByTeacherField : DTOServiceTests
    {
        [Fact]
        public async Task ShouldMarkAnswersByTeachers()
        {
            // Arrange
            var subjectId = Guid.NewGuid();

            var teacherUserName = "teacher1";

            var teachers = new List<User> { new() { UserName = teacherUserName } };

            var questionDTO = QuestionFactory.CreateQuestionDetailedDto(
                    Guid.NewGuid(), Guid.NewGuid());

            var answerDTOs = new List<AnswerDetailedDTO> {
                AnswerFactory.CreateAnswerDetailedDTO(Guid.NewGuid(), "value", teacherUserName, false, false),
                AnswerFactory.CreateAnswerDetailedDTO(Guid.NewGuid(), "value", "userName2", false, false)
            };

            questionDTO.Answers = answerDTOs;

            questionDTO.SubjectId = subjectId;

            _mockUserRepository
                .Setup(repo => repo.GetTeachersBySubjectIdAsync(questionDTO.SubjectId))
                .ReturnsAsync(teachers);

            // Act
            await _dtoService.UpdatingAnsweredByTeacherField(questionDTO);

            // Assert
            Assert.True(questionDTO.Answers.ElementAt(0).AnsweredByTeacher);
            Assert.False(questionDTO.Answers.ElementAt(1).AnsweredByTeacher);
        }
    }
}
