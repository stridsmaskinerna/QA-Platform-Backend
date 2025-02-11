using QAPlatformAPI.AcceptanceTests.Attributes;
using QAPlatformAPI.IntegrationTests;
using QAPlatformAPI.IntegrationTests.Controllers;

namespace QAPlatformAPI.AcceptanceTests.UserStories;

public class UnauthenticatedUser : UserStoryTestBase
{
    private const string _UNAUTH_USER_ESSE_1 = """
    UNAUTH-USER.ESSE.1: As an unauthenticated user who is not logged in,
    I want to see questions that are marked as public,
    so I can view and learn about the course even if
    I am not registered at the University.
    """;

    private const string _UNAUTH_USER_ESSE_2 = """
    UNAUTH-USER.ESSE.2: As an unauthenticated user with a valid LTU-email
    I want to register/login on the website to gain access
    to all Q&As. This ensures that only people associated
    with LTU gain access.
    """;

    public UnauthenticatedUser(QAPlatformAPIFactory<Program> factory) :
        base(factory)
    { }

    public class Essential : UnauthenticatedUser
    {
        public Essential(QAPlatformAPIFactory<Program> factory) :
            base(factory)
        { }


        [UserStory(_UNAUTH_USER_ESSE_1, Skip = "Test Not Implemented")]
        public async Task UNAUTH_USER_ESSE_1()
        {
            // Todo create simple question seeding
        }

        [UserStory(_UNAUTH_USER_ESSE_2)]
        public async Task UNAUTH_USER_ESSE_2()
        {
            await RunTestsAsync<AuthenticationControllerTests.Register>();
            await RunTestsAsync<AuthenticationControllerTests.Login>();
        }
    }
}
