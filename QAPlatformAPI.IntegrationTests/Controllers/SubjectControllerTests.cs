using System.Net;
using System.Net.Http.Json;
using Domain.DTO.Request;

namespace QAPlatformAPI.Integration.Controllers;

public class QuestionControllerTests : IntegrationTestBase
{
    public QuestionControllerTests(
        QAPlatformAPIFactory<Program> factory
    ) : base(factory)
    { }

    public class CreateSubject_Admin : QuestionControllerTests
    {
        public CreateSubject_Admin(
            QAPlatformAPIFactory<Program> factory
        ) : base(factory)
        {
            AuthenticateAsAdminAsync().Wait();
        }

        [Fact]
        public async Task ShouldReturn_Created()
        {
            // Arrange
            var requestBody = new SubjectForCreationDTO
            {
                Name = "Test subject 2.0",
                SubjectCode = "TestCode"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/subject/create", requestBody);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
    }
}
