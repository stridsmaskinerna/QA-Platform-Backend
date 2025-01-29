using Application.Services;
using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;



using System.Text;

///authentication/login

namespace Presentation.Controllers;

[Route("api/authentication/login")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<User> _userManager;

    public AuthenticationController(IConfiguration configuration, UserManager<User> usMan)
    {
        _configuration = configuration;
        _userManager = usMan;
    }
    public class AuthenticationRequestBody
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }



    [HttpPost("authenticate")]
    public async Task<ActionResult<string>> Authenticate(AuthenticationRequestBody authenticationRequestBody)
    {
        User? user = await ValidateUserCredential(authenticationRequestBody.Email, authenticationRequestBody.Password);
        if (user == null) return Unauthorized();

        string? sKey = (_configuration?["secretKey"]) ?? throw new ArgumentNullException("something went wrong");
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(sKey));
        var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var userRoles = await _userManager.GetRolesAsync(user);



        string rolesString = userRoles.Count > 0 ? string.Join(",", userRoles) : "";


        List<Claim> infoInToken =
        [
            new Claim("Username", user.UserName!),
            new Claim("Role", rolesString),
            new Claim("Email", user.Email!.ToString()),
            new Claim("Id", user.Id!.ToString()),
        ];

        var jwtSecurityToken = new JwtSecurityToken(
            _configuration["JwtSettings:Issuer"],
            _configuration["JwtSettings:Audience"],
            infoInToken,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:Expires"])),
            credential);

        var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);



        return Ok(tokenToReturn);
    }

    private async Task<User?> ValidateUserCredential(string? email, string? password)
    {
        var user = await _userManager.FindByEmailAsync(email!);

        if (user == null || !await _userManager.CheckPasswordAsync(user, password!)) return null;

        return user;

    }
}
