using Domain.DTO.Request;

namespace TestUtility.Factories;

public static class AuthenticationFactory
{
    public static AuthenticationDTO CreateAuthenticationDTO(
        string email = "test@test.com",
        string password = "TestPassword"
    ) => new()
    {
        Email = email,
        Password = password
    };

    public static RegistrationDTO CreateRegistrationDTO(
        string userName = "TesUser",
        string email = "test@test.com",
        string password = "TestPassword"
    ) => new()
    {
        UserName = userName,
        Email = email,
        Password = password
    };
}
