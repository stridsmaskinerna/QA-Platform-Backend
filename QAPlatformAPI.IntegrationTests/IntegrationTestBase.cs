using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Infrastructure.Seeds;

namespace QAPlatformAPI.Integration;

public class IntegrationTestBase : IClassFixture<QAPlatformAPIFactory<Program>>
{
    protected readonly HttpClient _client;
    protected readonly QAPlatformAPIFactory<Program> _factory;
    protected string _JWTToken = string.Empty;

    public IntegrationTestBase(QAPlatformAPIFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    protected async Task AuthenticateAsync(
        string email = SeedConstants.ADMIN_EMAIL,
        string password = SeedConstants.DEFAULT_PWD
    )
    {
        if (!string.IsNullOrEmpty(_JWTToken))
        {
            return;
        }

        var requestBody = new AuthenticationDTO
        {
            Email = email,
            Password = password
        };

        var response = await _client.PostAsJsonAsync(
            "/api/authentication/login",
            requestBody);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenDTO>();

        if (tokenResponse is null ||
            string.IsNullOrEmpty(tokenResponse.accessToken))
        {
            throw new InvalidOperationException("Failed to retrieve a valid JWT token.");
        }

        _JWTToken = tokenResponse.accessToken;

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            _JWTToken);
    }
}
