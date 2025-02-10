using Application.Services;
using Application.Tests.Utilities;
using Domain.Constants;
using Domain.DTO.Request;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Application.Tests.Services;

public class AuthenticationServiceTests : SetupServiceTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly AuthenticationService _authenticationService;

    public AuthenticationServiceTests()
    {
        _mockConfiguration = new Mock<IConfiguration>(MockBehavior.Strict);

        _mockConfiguration.Setup(c => c["secretKey"]).Returns("TestSecretWithAtLeast128BitsSize");
        _mockConfiguration.Setup(c => c["JwtSettings:Issuer"]).Returns("TestIssuer");
        _mockConfiguration.Setup(c => c["JwtSettings:Audience"]).Returns("TestAudience");
        _mockConfiguration.Setup(c => c["JwtSettings:Expires"]).Returns("60");

        _authenticationService = new AuthenticationService(
            _mockConfiguration.Object,
            _mockRepositoryManager.Object,
            _mockUserManager.Object);
    }

    public class Authenticate : AuthenticationServiceTests
    {
        private readonly AuthenticationDTO _authDto;
        private readonly User _user;

        public Authenticate()
        {
            _authDto = AuthenticationFactory.CreateAuthenticationDTO(
                "test@test.com",
                "AuthenticationPassword"
            );

            _user = UserFactory.CreateUser(
                "TestId",
                "TestUser",
                _authDto.Email ?? ""
            );
        }

        [Fact]
        public async Task ShouldReturnTokenDTO_WhenCredentialsAreValid()
        {
            // Arrange
            _mockUserRepository
                .Setup(r => r.ValidateUserCredential(_authDto.Email, _authDto.Password))
                .ReturnsAsync(_user);

            _mockUserManager
                .Setup(u => u.GetRolesAsync(_user))
                .ReturnsAsync(["User"]);

            // Act
            var result = await _authenticationService.Authenticate(_authDto);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.refreshToken);
            Assert.False(string.IsNullOrEmpty(result.accessToken));
        }

        [Fact]
        public async Task ShouldThrowUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            _mockUserRepository
                .Setup(r => r.ValidateUserCredential(_authDto.Email, _authDto.Password))
                .ReturnsAsync(default(User));

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(
                () => _authenticationService.Authenticate(_authDto));
        }
    }

    public class Register : AuthenticationServiceTests
    {
        private readonly RegistrationDTO registrationDto;

        public Register()
        {
            registrationDto = new()
            {
                UserName = "TestUser",
                Email = "test@test.com",
                Password = "TestPassword123!"
            };
        }

        [Fact]
        public async Task ShouldCreateUser_WhenRegistrationDataIsValid()
        {
            // Arrange
            _mockUserManager
                .Setup(u => u.FindByEmailAsync(registrationDto.Email))
                .ReturnsAsync(default(User));

            _mockUserManager
                .Setup(u => u.FindByNameAsync(registrationDto.UserName))
                .ReturnsAsync(default(User));

            _mockUserManager
                .Setup(u => u.CreateAsync(It.IsAny<User>(), registrationDto.Password))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager
                .Setup(u => u.AddToRoleAsync(It.IsAny<User>(), DomainRoles.USER))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _authenticationService.RegisterUser(registrationDto);

            // Assert
            _mockUserManager.Verify(
                u => u.CreateAsync(It.IsAny<User>(), registrationDto.Password),
                Times.Once);

            _mockUserManager.Verify(
                u => u.AddToRoleAsync(It.IsAny<User>(), DomainRoles.USER),
                Times.Once);
        }

        [Fact]
        public async Task ShouldThrowConflict_WhenEmailIsTaken()
        {
            // Arrange
            _mockUserManager
                .Setup(u => u.FindByEmailAsync(registrationDto.Email))
                .ReturnsAsync(new User());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ConflictException>(
                () => _authenticationService.RegisterUser(registrationDto));

            Assert.Equal(
                AuthenticationService.MsgConflictEmail(),
                exception.Message);
        }

        [Fact]
        public async Task ShouldThrowConflict_WhenUserNameIsTaken()
        {
            // Arrange
            _mockUserManager
                .Setup(u => u.FindByEmailAsync(registrationDto.Email))
                .ReturnsAsync(default(User));

            _mockUserManager
                .Setup(u => u.FindByNameAsync(registrationDto.UserName))
                .ReturnsAsync(new User());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ConflictException>(
                () => _authenticationService.RegisterUser(registrationDto));

            Assert.Equal(
                AuthenticationService.MsgConflictUserName(),
                exception.Message);
        }

        [Fact]
        public async Task ShouldThrowBadRequest_WhenUserCreationFails()
        {
            // Arrange
            var erroDescription = "Weak password";

            var identityErrors = new List<IdentityError> {
                new() { Description = erroDescription }
            };

            var failedResult = IdentityResult.Failed([.. identityErrors]);

            _mockUserManager
                .Setup(u => u.FindByEmailAsync(registrationDto.Email))
                .ReturnsAsync(default(User));

            _mockUserManager
                .Setup(u => u.FindByNameAsync(registrationDto.UserName))
                .ReturnsAsync(default(User));

            _mockUserManager
                .Setup(u => u.CreateAsync(It.IsAny<User>(), registrationDto.Password))
                .ReturnsAsync(failedResult);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(
                () => _authenticationService.RegisterUser(registrationDto));

            Assert.Equal(erroDescription, exception.Message);
        }
    }
}
