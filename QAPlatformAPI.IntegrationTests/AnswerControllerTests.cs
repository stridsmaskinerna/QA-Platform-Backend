using System.Net;
using System.Net.Http.Json;
using Domain.DTO.Request;

namespace QAPlatformAPI.IntegrationTests;


public class AnswerControllerTests : IClassFixture<QAPlatformAPIFactory<Program>>
{
    private readonly HttpClient _client;

    public AnswerControllerTests(QAPlatformAPIFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("TestAuthScheme");
    }

    [Fact]
    public async Task CreateAnswer_ShouldReturn_Created()
    {
        // Arrange
        var requestBody = new AnswerForCreationDTO
        {
            Value = "Test answer"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/answers", requestBody);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
