using System.Diagnostics.CodeAnalysis;
using Domain.Exceptions;

namespace Application.Services;

public class BaseService : IBaseService
{
    [DoesNotReturn]
    public void BadRequest(
        string detail = "",
        string title = "Bad Request"
    ) => throw new BadRequestException(detail, title);

    [DoesNotReturn]
    public void Conflict(
        string detail = "",
        string title = "Conflict"
    ) => throw new ConflictException(detail, title);

    [DoesNotReturn]
    public void Forbidden(
        string detail = "",
        string title = "Forbidden"
    ) => throw new ForbiddenException(detail, title);

    [DoesNotReturn]
    public void NotFound(
        string detail = "",
        string title = "Not Found"
    ) => throw new NotFoundException(detail, title);

    [DoesNotReturn]
    public void Unauthorized(
        string detail = "",
        string title = "Unauthorized"
    ) => throw new UnauthorizedException(detail, title);
}
