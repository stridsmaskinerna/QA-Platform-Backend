using QAPlatformAPI.AcceptanceTests.Attributes;
using QAPlatformAPI.IntegrationTests;
using QAPlatformAPI.IntegrationTests.Controllers;

namespace QAPlatformAPI.AcceptanceTests.UserStories;

public class Admin : UserStoryTestBase
{
    private const string _ADMIN_ESSE_1 = """
    ADMIN.ESSE.1: As an administrator, I want to set up course codes before any questions
    can be posted, so that structure is clearer and no questions can be asked
    without connection to a course
    """;

    public Admin(QAPlatformAPIFactory<Program> factory) :
        base(factory)
    { }

    public class Essential : Admin
    {
        public Essential(QAPlatformAPIFactory<Program> factory) :
            base(factory)
        { }

        [UserStory(_ADMIN_ESSE_1)]
        public async Task ADMIN_ESSE_1()
        {
            await RunTestsAsync<QuestionControllerTests.CreateSubject>();
        }
    }
}
