using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.DTO.Request;
using Domain.Constants;
using Domain.DTO.Response;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services;

public class AuthenticationService : BaseService, IAuthenticationService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<User> _userManager;

    public AuthenticationService(IConfiguration configuration, UserManager<User> usMan)
    {
        _configuration = configuration;
        _userManager = usMan;
    }

    public async Task<TokenDTO> Authenticate(
        AuthenticationDTO authenticationDTO
    )
    {
        User? user = await ValidateUserCredential(authenticationDTO.Email, authenticationDTO.Password);
        if (user == null)
        {
            // Throw Unauthorized Exception handled by ExceptionMiddleware.
            Unauthorized();
        };

        string? sKey = (_configuration?["secretKey"]) ?? throw new ArgumentNullException("something went wrong");
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(sKey));
        var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var userRoles = await _userManager.GetRolesAsync(user);

        List<Claim> infoInToken =
        [
            new Claim(DomainClaims.USER_NAME, user.UserName!),
            new Claim(DomainClaims.EMAIL, user.Email!.ToString()),
            new Claim(DomainClaims.USER_ID, user.Id!.ToString()),
        ];

        // ✅ Store each role as a separate claim
        foreach (var role in userRoles)
        {
            infoInToken.Add(new Claim(ClaimTypes.Role, role));
        }

        var jwtSecurityToken = new JwtSecurityToken(
            _configuration["JwtSettings:Issuer"],
            _configuration["JwtSettings:Audience"],
            infoInToken,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:Expires"])),
            credential);

        var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        return new TokenDTO
        {
            accessToken = tokenToReturn,
            refreshToken = ""
        };
    }
    public async Task<TokenDTO> RegisterUser(RegistrationDTO registrationDTO)
    {

        var existingUser = await _userManager.FindByEmailAsync(registrationDTO.Email);
        if (existingUser != null)
        {
            throw new ArgumentException("A user with the same email already exists.");
        }


        var user = new User
        {
            UserName = registrationDTO.UserName,
            Email = registrationDTO.Email
        };


        var result = await _userManager.CreateAsync(user, registrationDTO.Password);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        if (registrationDTO.Roles != null && registrationDTO.Roles.Any())
        {
            await _userManager.AddToRolesAsync(user, registrationDTO.Roles);
        }

        string? sKey = _configuration["secretKey"] ?? throw new ArgumentNullException("Secret key is missing.");
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(sKey));
        var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var userRoles = await _userManager.GetRolesAsync(user);
        string rolesString = userRoles.Count > 0 ? string.Join(",", userRoles) : "";

        List<Claim> infoInToken = new()
        {
            new Claim("username", user.UserName!),
            new Claim("roles", rolesString),
            new Claim("email", user.Email!.ToString()),
            new Claim("userId", user.Id!.ToString()),
        };

        var jwtSecurityToken = new JwtSecurityToken(
            _configuration["JwtSettings:Issuer"],
            _configuration["JwtSettings:Audience"],
            infoInToken,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:Expires"])),
            credential);

        var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        return new TokenDTO
        {
            accessToken = tokenToReturn,
            refreshToken = ""
        };
    }
    private async Task<User?> ValidateUserCredential(string? email, string? password)
    {
        var user = await _userManager.FindByEmailAsync(email!);

        if (user == null || !await _userManager.CheckPasswordAsync(user, password!)) return null;

        return user;

    }
}
