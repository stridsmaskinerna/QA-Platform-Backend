using QAPlatformAPI.AcceptanceTests.Attributes;
using QAPlatformAPI.IntegrationTests;
using QAPlatformAPI.IntegrationTests.Controllers;

namespace QAPlatformAPI.AcceptanceTests.UserStories;

public class Admin : UserStoryTestBase
{
    public Admin(QAPlatformAPIFactory<Program> factory) :
        base(factory)
    { }

    public class Essential : Admin
    {
        public Essential(QAPlatformAPIFactory<Program> factory) :
            base(factory)
        { }

        [UserStory(Description.ADMIN_ESSE_1, Skip = "Unknown failure after reafactorin")]
        public async Task ADMIN_ESSE_1()
        {
            await RunTestsAsync<QuestionControllerTests.CreateSubject>();
        }

        [UserStory(Description.ADMIN_ESSE_2, Skip = "Not implemented and tested")]
        public async Task ADMIN_ESSE_2()
        {
        }

        [UserStory(Description.ADMIN_ESSE_3, Skip = "Not implemented and tested")]
        public async Task ADMIN_ESSE_3()
        {
        }
    }

    public class Desirable : Admin
    {
        public Desirable(QAPlatformAPIFactory<Program> factory) :
            base(factory)
        { }

        [UserStory(Description.ADMIN_DESIRABLE_1, Skip = "Not implemented and tested")]
        public async Task ADMIN_DESIRABLE_1()
        {
        }

        [UserStory(Description.ADMIN_DESIRABLE_2, Skip = "Not implemented and tested")]
        public async Task ADMIN_DESIRABLE_2()
        {
            await RunTestsAsync<QuestionControllerTests.CreateSubject>();
        }
    }
}

file class Description
{
    #region User Stories Essential

    internal const string ADMIN_ESSE_1 = """
    ADMIN.ESSE.1: As an administrator,
    I want to set up course codes before any questions can be posted,
    so that structure is clearer and no questions can be asked
    without connection to a course
    """;

    internal const string ADMIN_ESSE_2 = """
    ADMIN.ESSE.2: As an administrator,
    I want to assign users as teachers for specific courses so I
    can grant users access to specific course Q&A
    """;

    internal const string ADMIN_ESSE_3 = """
    ADMIN.ESSE.3: As an administrator,
    I want to be able to deactivate / block a user to prevent
    disruptive users from accessing Q&A or after course completion
    """;

    #endregion User Stories Essential

    #region User Stories Desirable

    internal const string ADMIN_DESIRABLE_1 = """
    ADMIN.DESIRABLE.1: As an administrator,
    I want to be able to block tags created by Users in case
    some user adds an offensive word as a tag.
    """;

    internal const string ADMIN_DESIRABLE_2 = """
    ADMIN.DESIRABLE.2: As an administrator,
    I want to be able to see who created tags in order to keep track of
    anyone repeatedly adding offensive words as tags and
    then be able to block them
    """;

    #endregion User Stories Desirable

}
