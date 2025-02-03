using System.Security.Claims;
using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Services;

public class TokenService : BaseService, ITokenService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetUserName()
    {
        var userName = GetUser().FindFirst(DomainClaims.USER_NAME)?.Value;

        if (userName == null)
        {
            Unauthorized();
        }

        return userName;
    }

    public string GetUserId()
    {
        var userId = GetUser().FindFirst(DomainClaims.USER_ID)?.Value;

        if (userId == null)
        {
            Unauthorized();
        }

        return userId;
    }

    private ClaimsPrincipal GetUser()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user == null)
        {
            Unauthorized();
        }

        return user;
    }
}
