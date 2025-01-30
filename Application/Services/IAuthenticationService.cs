using Domain.DTO.Request;
using Domain.DTO.Response;

namespace Application.Services;

public interface IAuthenticationService
{
    Task<TokenDTO> Authenticate(AuthenticationDTO authenticationRequestBody);
    Task<TokenDTO> RegisterUser(RegistrationDTO registration);
}
