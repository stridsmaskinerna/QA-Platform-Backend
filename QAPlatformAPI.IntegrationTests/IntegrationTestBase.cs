using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Domain.Constants;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Infrastructure.Seeds.Test;

namespace QAPlatformAPI.IntegrationTests;

public class IntegrationTestBase : IClassFixture<QAPlatformAPIFactory<Program>>
{
    protected readonly HttpClient _client;
    protected readonly QAPlatformAPIFactory<Program> _factory;
    private (string Token, string Role) _tokenContainer = (string.Empty, string.Empty);

    public IntegrationTestBase(QAPlatformAPIFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    protected async Task AuthenticateAsAdminAsync()
    {
        await AuthenticateAsync(
            SeedDataTest.ADMIN_EMAIL,
            SeedDataTest.DEFAULT_PWD,
            DomainRoles.ADMIN);
    }

    protected async Task AuthenticateAsTeacherAsync()
    {
        await AuthenticateAsync(
            SeedDataTest.TEACHER_EMAIL,
            SeedDataTest.DEFAULT_PWD,
            DomainRoles.TEACHER);
    }

    protected async Task AuthenticateAsUserAsync()
    {
        await AuthenticateAsync(
            SeedDataTest.USER_EMAIL,
            SeedDataTest.DEFAULT_PWD,
            DomainRoles.USER);
    }

    private async Task AuthenticateAsync(
        string email,
        string password,
        string role
    )
    {
        if (
            !string.IsNullOrEmpty(_tokenContainer.Token) &&
            _tokenContainer.Role == role
        )
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

        _tokenContainer = (Token: tokenResponse.accessToken, Role: role);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            _tokenContainer.Token);
    }
}
