using System.Net;
using System.Net.Http.Json;
using Domain.DTO.Request;
using Infrastructure.Seeds.Test;

namespace QAPlatformAPI.IntegrationTests.Controllers;

public class AuthenticationControllerTests : IntegrationTestBase
{
    public AuthenticationControllerTests(QAPlatformAPIFactory<Program> factory) :
        base(factory)
    { }

    public class Register : AuthenticationControllerTests
    {
        private const string _endpoint = "/api/authentication/register";

        public Register(QAPlatformAPIFactory<Program> factory) :
            base(factory)
        { }

        [Fact]
        public async Task ShouldReturn_Ok_WhenValidRegistrationData()
        {
            // Arrange
            var requestBody = new RegistrationDTO
            {
                Email = "123TestUser@ltu.se",
                UserName = "123newTestUser",
                Password = "password"
            };

            // Act
            var response = await _client.PostAsJsonAsync(_endpoint, requestBody);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(Skip = "Not Implemented")]
        public async Task ShouldReturn_BadRequest_WhenInvalidMail()
        {
            // Arrange
            var requestBody = new RegistrationDTO
            {
                Email = "321TestUser@mail.com",
                UserName = "321newTestUser",
                Password = "password"
            };

            // Act
            var response = await _client.PostAsJsonAsync(_endpoint, requestBody);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ShouldReturn_BadRequest_WhenInvalidPassword()
        {
            // Arrange
            var requestBody = new RegistrationDTO
            {
                Email = "321TestUser@mail.com",
                UserName = "321newTestUser",
                Password = "p"
            };

            // Act
            var response = await _client.PostAsJsonAsync(_endpoint, requestBody);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ShouldReturn_Conflict_WhenRegistrationDataIsConflict()
        {
            // Arrange
            var requestBody = new RegistrationDTO
            {
                Email = SeedConstantsTest.USER_EMAIL,
                UserName = SeedConstantsTest.USER_USERNAME,
                Password = SeedConstantsTest.DEFAULT_PWD
            };

            // Act
            var response = await _client.PostAsJsonAsync(_endpoint, requestBody);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }
    }

    public class Login : AuthenticationControllerTests
    {
        private const string _endpoint = "/api/authentication/login";

        public Login(QAPlatformAPIFactory<Program> factory) :
            base(factory)
        { }

        [Fact]
        public async Task ShouldReturn_Ok_WhenValidRegistrationData()
        {
            // Arrange
            var requestBody = new RegistrationDTO
            {
                Email = SeedConstantsTest.USER_EMAIL,
                UserName = SeedConstantsTest.USER_USERNAME,
                Password = SeedConstantsTest.DEFAULT_PWD
            };

            // Act
            var response = await _client.PostAsJsonAsync(_endpoint, requestBody);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ShouldReturn_Unauthorized_WhenInvalidRegistrationData()
        {
            // Arrange
            var requestBody = new RegistrationDTO
            {
                Email = "UnknownTestUser@mail.com",
                UserName = "UnknownnewUser",
                Password = "password"
            };

            // Act
            var response = await _client.PostAsJsonAsync(_endpoint, requestBody);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
