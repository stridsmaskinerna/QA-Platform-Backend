using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Services;
using Domain.DTO.Response;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

///authentication/login

namespace Presentation.Controllers;

[ApiController]
[Route("api/authentication/")]
[Produces("application/json")]
public class AuthenticationController : ControllerBase
{
    private readonly IServiceManager _sm;

    public AuthenticationController(IServiceManager sm)
    {
        _sm = sm;
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokenDTO>> Authenticate(
        AuthenticationDTO authenticationRequestBody
    )
    {
        var token = await _sm.AuthenticationService.Authenticate(authenticationRequestBody);

        return Ok(token);
    }
}
