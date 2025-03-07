using Application.Contracts;
using Domain.Constants;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokenDTO>> Authenticate(
        AuthenticationDTO authenticationDTO
    )
    {
        var token = await _sm.AuthenticationService.Authenticate(authenticationDTO);

        return Ok(token);
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TokenDTO>> RegisterUser(RegistrationDTO registrationDTO)
    {
        await _sm.AuthenticationService.RegisterUser(registrationDTO);
        return Ok();
    }

    [Authorize(Roles = DomainRoles.USER)]
    [HttpGet("debug-claims")]
    public IActionResult DebugRoles()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        return Ok(claims);
    }
}
