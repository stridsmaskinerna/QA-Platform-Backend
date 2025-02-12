using System.Net;
using System.Net.Http.Json;
using Infrastructure.Seeds.Test;
using TestUtility.Factories;

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

        [Theory]
        [InlineData("12newTestUser", "password")]
        [InlineData("123newTestUser", "PASSWORD")]
        [InlineData("1234newTestUser", "12345678")]
        [InlineData("12335newTestUser", "00000000")]
        public async Task ShouldReturn_Ok_WhenValidRegistrationData(
            string userName,
            string password
        )
        {
            // Arrange
            var requestBody = AuthenticationFactory.CreateRegistrationDTO(
                userName,
                $"{userName}@ltu.se",
                password
            );

            // Act
            var response = await _client.PostAsJsonAsync(_endpoint, requestBody);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(Skip = "Not Implemented")]
        public async Task ShouldReturn_BadRequest_WhenInvalidMail()
        {
            // Arrange
            var requestBody = AuthenticationFactory.CreateRegistrationDTO(
                "InavalidMailUser",
                "InavalidMailUser@mail.com",
                "password"
            );

            // Act
            var response = await _client.PostAsJsonAsync(_endpoint, requestBody);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("p")]
        [InlineData("pa")]
        [InlineData("pass")]
        [InlineData("passwor")]
        public async Task ShouldReturn_BadRequest_WhenInvalidPassword(
            string password
        )
        {
            // Arrange
            var requestBody = AuthenticationFactory.CreateRegistrationDTO(
                "InvalidPasswordUser",
                "InvalidPasswordUser@mail.com",
                password
            );

            // Act
            var response = await _client.PostAsJsonAsync(_endpoint, requestBody);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ShouldReturn_Conflict_WhenRegistrationDataIsConflict()
        {
            // Arrange
            var requestBody = AuthenticationFactory.CreateRegistrationDTO(
                SeedConstantsTest.USER_USERNAME,
                SeedConstantsTest.USER_EMAIL,
                SeedConstantsTest.DEFAULT_PWD
            );

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
            var requestBody = AuthenticationFactory.CreateRegistrationDTO(
                SeedConstantsTest.USER_USERNAME,
                SeedConstantsTest.USER_EMAIL,
                SeedConstantsTest.DEFAULT_PWD
            );

            // Act
            var response = await _client.PostAsJsonAsync(_endpoint, requestBody);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ShouldReturn_Unauthorized_WhenInvalidRegistrationData()
        {
            // Arrange
            var requestBody = AuthenticationFactory.CreateRegistrationDTO(
                "UnknownnewUser",
                "UnknownTestUser@mail.com",
                "password"
            );

            // Act
            var response = await _client.PostAsJsonAsync(_endpoint, requestBody);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
