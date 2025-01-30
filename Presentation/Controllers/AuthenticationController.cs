using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Services;
using Domain.DTO.Request;
using Domain.Constants;
using Domain.DTO.Response;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
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
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TokenDTO>> RegisterUser(RegistrationDTO registrationDTO)
    {
        try
        {
            var token = await _sm.AuthenticationService.RegisterUser(registrationDTO);

            return Ok(token);
        }
        catch (ArgumentException ex)
        {
            if (ex.Message.Contains("User already exists"))
            {
                return Conflict(new { Message = ex.Message });
            }

            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
        }
    }

    [Authorize(Roles = Roles.USER)]
    [HttpGet("debug-claims")]
    public IActionResult DebugRoles()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        return Ok(claims);
    }
}
