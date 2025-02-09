using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Domain.DTO.Request;
using Domain.DTO.Response;

namespace QAPlatformAPI.Integration.Controllers;

public class QuestionControllerTests : IntegrationTestBase
{
    public QuestionControllerTests(
        QAPlatformAPIFactory<Program> factory
    ) : base(factory)
    {
        AuthenticateAsync().Wait();
    }

    [Fact]
    public async Task CreateSubject_ShouldReturn_Created()
    {
        await AuthenticateAsync();

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
