using Domain.Entities;

namespace Application.Tests.Utilities;

public static class UserFactory
{
    public static User CreateUser(
        string id,
        string userName
    ) => new()
    {
        Id = id,
        UserName = userName
    };
}
