using System.Security.Claims;
using Application.Services;
using Domain.Constants;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Application.Tests.Services;

public class TokenServiceTests
{
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly TokenService _tokenService;
    private readonly DefaultHttpContext _mockHttpContext;
    private readonly ClaimsPrincipal _claimsPrincipal;

    public TokenServiceTests()
    {
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>(MockBehavior.Strict);
        _mockHttpContext = new DefaultHttpContext();
        _claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());

        // Setup Default HttpContext with no claims
        _mockHttpContext.User = _claimsPrincipal;
        _mockHttpContextAccessor
            .Setup(httpContextAccessor => httpContextAccessor.HttpContext)
            .Returns(_mockHttpContext);

        _tokenService = new TokenService(_mockHttpContextAccessor.Object);
    }

    public class GetUserName : TokenServiceTests
    {

        [Fact]
        public void ShouldReturnUserName_WhenUserExists()
        {
            // Arrange
            var expectedUserName = "expectedUserName";
            _mockHttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(DomainClaims.USER_NAME, expectedUserName)
            ]));

            // Act
            var result = _tokenService.GetUserName();

            // Assert
            Assert.Equal(expectedUserName, result);
        }

        [Fact]
        public void ShouldThrowUnauthorized_WhenClaimUserNameIsMissing()
        {
            // Act & Assert
            Assert.Throws<UnauthorizedException>(_tokenService.GetUserName);
        }

        [Fact]
        public void ShouldThrowUnauthorized_WhenHttpContextIsNull()
        {
            // Arrange
            _mockHttpContextAccessor
                .Setup(a => a.HttpContext)
                .Returns(default(HttpContext)!);

            // Act & Assert
            Assert.Throws<UnauthorizedException>(_tokenService.GetUserName);
        }
    }

    public class GetUserId : TokenServiceTests
    {

        [Fact]
        public void ShouldReturnUserId_WhenUserExists()
        {
            // Arrange
            var expectedUserId = "expectedUserId";
            _mockHttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(DomainClaims.USER_ID, expectedUserId)
            ]));

            // Act
            var result = _tokenService.GetUserId();

            // Assert
            Assert.Equal(expectedUserId, result);
        }

        [Fact]
        public void ShouldThrowUnauthorized_WhenClaimUserIdIsMissing()
        {
            // Act & Assert
            Assert.Throws<UnauthorizedException>(_tokenService.GetUserId);
        }

        [Fact]
        public void ShouldThrowUnauthorized_WhenHttpContextIsNull()
        {
            // Arrange
            _mockHttpContextAccessor
                .Setup(a => a.HttpContext)
                .Returns(default(HttpContext)!);

            // Act & Assert
            Assert.Throws<UnauthorizedException>(_tokenService.GetUserName);
        }
    }
}

