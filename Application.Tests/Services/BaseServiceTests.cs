using Application.Services;
using Domain.Exceptions;

namespace Application.Tests.Services;

public class BaseServiceTests
{
    private readonly BaseService _baseService;

    public BaseServiceTests()
    {
        _baseService = new BaseService();
    }

    [Theory]
    [InlineData("Bad request occurred", typeof(BadRequestException))]
    [InlineData("Conflict detected", typeof(ConflictException))]
    [InlineData("Access denied", typeof(ForbiddenException))]
    [InlineData("Resource not found", typeof(NotFoundException))]
    [InlineData("Unauthorized access", typeof(UnauthorizedException))]
    public void ExceptionMethods_ShouldThrowCorrectException(
        string detail,
        Type expectedExceptionType
    )
    {
        // Act & Assert
        var exception = Assert.Throws(expectedExceptionType,
            () => CallExceptionMethod(detail, expectedExceptionType));

        Assert.Equal(detail, exception.Message);
    }

    private void CallExceptionMethod(string detail, Type exceptionType)
    {
        switch (exceptionType.Name)
        {
            case nameof(BadRequestException):
                _baseService.BadRequest(detail);
                break;
            case nameof(ConflictException):
                _baseService.Conflict(detail);
                break;
            case nameof(ForbiddenException):
                _baseService.Forbidden(detail);
                break;
            case nameof(NotFoundException):
                _baseService.NotFound(detail);
                break;
            case nameof(UnauthorizedException):
                _baseService.Unauthorized(detail);
                break;
            default:
                throw new NotImplementedException();
        }
    }
}

