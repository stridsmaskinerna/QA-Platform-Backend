using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Contracts;
using Domain.Constants;
using Domain.Contracts;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services;

public class AuthenticationService : BaseService, IAuthenticationService
{
    private readonly IConfiguration _configuration;
    private readonly IRepositoryManager _rm;
    private readonly UserManager<User> _userManager;

    public AuthenticationService(
        IConfiguration configuration,
        IRepositoryManager rm,
        UserManager<User> usMan
    )
    {
        _configuration = configuration;
        _rm = rm;
        _userManager = usMan;
    }

    public static string MsgConflictEmail() => "Email taken";

    public static string MsgConflictUserName() => "Username taken";

    public async Task<TokenDTO> Authenticate(
        AuthenticationDTO authenticationDTO
    )
    {
        User? user = await _rm.UserRepository.ValidateUserCredential(
            authenticationDTO.Email, authenticationDTO.Password);

        if (user == null)
        {
            // Throw Unauthorized Exception handled by ExceptionMiddleware.
            Unauthorized();
        }

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

        // Store each role as a separate claim
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

    public async Task RegisterUser(RegistrationDTO registrationDTO)
    {

        var userWithEmail = await _userManager.FindByEmailAsync(registrationDTO.Email);
        if (userWithEmail != null)
        {
            Conflict(MsgConflictEmail());
        }

        var userWithUsername = await _userManager.FindByNameAsync(registrationDTO.UserName);
        if (userWithUsername != null)
        {
            Conflict(MsgConflictUserName());
        }

        var user = new User
        {
            UserName = registrationDTO.UserName,
            Email = registrationDTO.Email
        };

        var result = await _userManager.CreateAsync(user, registrationDTO.Password);

        if (!result.Succeeded)
        {
            BadRequest(string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        string? sKey = _configuration["secretKey"] ?? throw new ArgumentNullException("Secret key is missing.");
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(sKey));
        var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        await _userManager.AddToRoleAsync(user, DomainRoles.USER);
    }
}
