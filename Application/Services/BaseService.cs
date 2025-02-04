using System.Diagnostics.CodeAnalysis;
using Application.Contracts;
using Domain.Exceptions;

namespace Application.Services;

public class BaseService : IBaseService
{
    [DoesNotReturn]
    public void BadRequest(
        string detail = ""
    ) => throw new BadRequestException(detail);

    [DoesNotReturn]
    public void Conflict(
        string detail = ""
    ) => throw new ConflictException(detail);

    [DoesNotReturn]
    public void Forbidden(
        string detail = ""
    ) => throw new ForbiddenException(detail);

    [DoesNotReturn]
    public void NotFound(
        string detail = ""
    ) => throw new NotFoundException(detail);

    [DoesNotReturn]
    public void Unauthorized(
        string detail = ""
    ) => throw new UnauthorizedException(detail);
}
