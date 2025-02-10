using Domain.Entities;

namespace Application.Tests.Utilities;

public static class UserFactory
{
    public static User CreateUser(
        string id,
        string userName,
        string email = "example@example.com"
    ) => new()
    {
        Id = id,
        UserName = userName,
        Email = email
    };
}
