using Domain.DTO.Request;
using Domain.DTO.Response;

namespace Application.Contracts;

public interface IAuthenticationService
{
    Task<TokenDTO> Authenticate(AuthenticationDTO authenticationRequestBody);
    Task RegisterUser(RegistrationDTO registration);
}
