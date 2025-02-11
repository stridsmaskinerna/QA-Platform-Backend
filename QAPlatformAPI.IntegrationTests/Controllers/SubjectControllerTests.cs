using System.Net;
using System.Net.Http.Json;
using Domain.DTO.Request;

namespace QAPlatformAPI.IntegrationTests.Controllers;


public class QuestionControllerTests : IntegrationTestBase
{
    public QuestionControllerTests(QAPlatformAPIFactory<Program> factory) :
        base(factory)
    { }

    public class CreateSubject : QuestionControllerTests
    {
        private const string _endpoint = "/api/subject/create";

        public CreateSubject(QAPlatformAPIFactory<Program> factory) :
            base(factory)
        { }

        [Fact]
        public async Task ShouldReturn_Created_WhenUserHasRoleAdmin()
        {
            // Arrange
            await AuthenticateAsAdminAsync();
            var requestBody = new SubjectForCreationDTO
            {
                Name = "Test subject 2.0",
                SubjectCode = "TestCode"
            };

            // Act
            var response = await _client.PostAsJsonAsync(_endpoint, requestBody);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task ShouldReturn_Forbidden_WhenUserHasRoleTeacher()
        {
            // Arrange
            await AuthenticateAsTeacherAsync();
            var requestBody = new SubjectForCreationDTO
            {
                Name = "Test subject 2.0",
                SubjectCode = "TestCode"
            };

            // Act
            var response = await _client.PostAsJsonAsync(_endpoint, requestBody);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task ShouldReturn_Forbidden_WhenUserHasRoleUser()
        {
            // Arrange
            await AuthenticateAsUserAsync();
            var requestBody = new SubjectForCreationDTO
            {
                Name = "Test subject 2.0",
                SubjectCode = "TestCode"
            };

            // Act
            var response = await _client.PostAsJsonAsync(_endpoint, requestBody);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
