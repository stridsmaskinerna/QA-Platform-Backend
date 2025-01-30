using System.Security.Claims;
using Domain.Constants;
using Microsoft.AspNetCore.Http;

namespace Application.Services;

public class TokenService : ITokenService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? GetUser()
    {
        return _httpContextAccessor.HttpContext?.User;
    }

    public string? GetUserName()
    {
        return GetUser()?.FindFirst(DomainClaims.USER_NAME)?.Value;
    }

    public string? GetUserId()
    {
        return GetUser()?.FindFirst(DomainClaims.USER_ID)?.Value;
    }
}
