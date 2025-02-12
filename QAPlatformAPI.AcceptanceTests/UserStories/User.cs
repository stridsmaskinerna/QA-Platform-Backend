using QAPlatformAPI.AcceptanceTests.Attributes;
using QAPlatformAPI.IntegrationTests;

namespace QAPlatformAPI.AcceptanceTests.UserStories;

public class User : UserStoryTestBase
{
    public User(QAPlatformAPIFactory<Program> factory) :
    base(factory)
    { }

    public class Essential : User
    {
        public Essential(QAPlatformAPIFactory<Program> factory) :
            base(factory)
        { }

        [UserStory(Description.USER_ESSE_1, Skip = "Not tested")]
        public async Task USER_ESSE_1()
        {

        }

        [UserStory(Description.USER_ESSE_2, Skip = "Not tested")]
        public async Task USER_ESSE_2()
        {

        }

        [UserStory(Description.USER_ESSE_3, Skip = "Not tested")]
        public async Task USER_ESSE_3()
        {

        }

        [UserStory(Description.USER_ESSE_4, Skip = "Not tested")]
        public async Task USER_ESSE_4()
        {

        }

        [UserStory(Description.USER_ESSE_5, Skip = "Not tested")]
        public async Task USER_ESSE_5()
        {

        }

        [UserStory(Description.USER_ESSE_6, Skip = "Not tested")]
        public async Task USER_ESSE_6()
        {

        }

        [UserStory(Description.USER_ESSE_7, Skip = "Not tested")]
        public async Task USER_ESSE_7()
        {

        }

        [UserStory(Description.USER_ESSE_8, Skip = "Not tested")]
        public async Task USER_ESSE_8()
        {

        }

        [UserStory(Description.USER_ESSE_9, Skip = "Not tested")]
        public async Task USER_ESSE_9()
        {

        }

        [UserStory(Description.USER_ESSE_10, Skip = "Not tested")]
        public async Task USER_ESSE_10()
        {

        }

        [UserStory(Description.USER_ESSE_11, Skip = "Not tested")]
        public async Task USER_ESSE_11()
        {

        }
    }

    public class Desirable : User
    {
        public Desirable(QAPlatformAPIFactory<Program> factory) :
            base(factory)
        { }

        [UserStory(Description.USER_DESIRABLE_1, Skip = "Not implemented and tested")]
        public async Task USER_DESIRABLE_1()
        {

        }

        [UserStory(Description.USER_DESIRABLE_2, Skip = "Not implemented and tested")]
        public async Task USER_DESIRABLE_2()
        {

        }

        [UserStory(Description.USER_DESIRABLE_3, Skip = "Not tested")]
        public async Task USER_DESIRABLE_3()
        {

        }

        [UserStory(Description.USER_DESIRABLE_4, Skip = "Not tested")]
        public async Task USER_DESIRABLE_4()
        {

        }

        [UserStory(Description.USER_DESIRABLE_5, Skip = "Not tested")]
        public async Task USER_DESIRABLE_5()
        {

        }

        [UserStory(Description.USER_DESIRABLE_6, Skip = "Not Applicable Back-end, Cat maybe test storage here.")]
        public async Task USER_DESIRABLE_6()
        {

        }

        [UserStory(Description.USER_DESIRABLE_7, Skip = "Not tested")]
        public async Task USER_DESIRABLE_7()
        {

        }

        [UserStory(Description.USER_DESIRABLE_8, Skip = "Not implemented and tested")]
        public async Task USER_DESIRABLE_8()
        {

        }

        [UserStory(Description.USER_DESIRABLE_9, Skip = "Not tested")]
        public async Task USER_DESIRABLE_9()
        {

        }
    }
}

file class Description
{
    #region User Stories Essential
    internal const string USER_ESSE_1 = """
    AUTH-USER.ESSE.1: As a user,
    I want to be able to see a list of questions in a course.
    So that I can see if anyone has asked something that I’m
    wondering about or something that I can answer.
    """;

    internal const string USER_ESSE_2 = """
    AUTH-USER.ESSE.2: As a user,
    I want to be able to ask and delete questions to help me fill
    my knowledge gaps in order to succeed in completing my courses.
    """;

    internal const string USER_ESSE_3 = """
    AUTH-USER.ESSE.3: As a user,
    I want to be able to tag (with keywords) questions I asked to
    make the question easier to search for.
    """;

    internal const string USER_ESSE_4 = """
    AUTH-USER.ESSE.4: As a user,
    I want to be able to post and delete answers to questions to
    help fellow users.
    """;

    internal const string USER_ESSE_5 = """
    AUTH-USER.ESSE.5: As a user,
    I want to be able to see the answers to questions to get information
    about what I need.
    """;

    internal const string USER_ESSE_6 = """
    AUTH-USER.ESSE.6: As a user,
    I want to be able to upvote or downvote answers to help fellow users
    to help surface the most useful responses.
    """;

    internal const string USER_ESSE_7 = """
    AUTH-USER.ESSE.7: As a user,
    I want to be able to comment and delete my comments on answers in
    order to clarify or add supplementary
    information.
    """;

    internal const string USER_ESSE_8 = """
    AUTH-USER.ESSE.8: As a user,
    I want to be able to search Q&A threads by tags, course code or
    suggested topic (for a specific course) in order to quickly find
    information about what I want.
    """;

    internal const string USER_ESSE_9 = """
    AUTH-USER.ESSE.9: As a user,
    I want every question to be associated with a specific course
    (or “general”-category) in order to help me find the information
    I’m looking for more easily.
    """;

    internal const string USER_ESSE_10 = """
    AUTH-USER.ESSE.10: As a user,
    I want to be able to set my posted question as either public or
    protected, to have control over who can see my question
    (public everyone can see, protected only logged in users).
    """;

    internal const string USER_ESSE_11 = """
    AUTH-USER.ESSE.11: As a user, I want to be able to quickly find Q&As that I have interacted with.
    """;

    #endregion User Stories Essential

    #region User Stories Desirable

    internal const string USER_DESIRABLE_1 = """
    AUTH-USER.DESIRABLE.1: As a user,
    I want to be able to upload supporting files of different types
    when posting questions.
    """;

    internal const string USER_DESIRABLE_2 = """
    AUTH-USER.DESIRABLE.2: As a user,
    I want to be able to upload supporting files of different types when
    posting answers.
    """;

    internal const string USER_DESIRABLE_3 = """
    AUTH-USER.DESIRABLE.3: As a user,
    I want to be able to view questions sorted by recent and by views.
    So that I can see the most relevant questions.
    """;

    internal const string USER_DESIRABLE_4 = """
    AUTH-USER.DESIRABLE.4: As a user,
    I want to be able to search Q&As by free text search (free text
    meaning question title, course name, course description etc).
    So that I can navigate to that course and see the questions related
    to that course.
    """;

    internal const string USER_DESIRABLE_5 = """
    AUTH-USER.DESIRABLE.5: As a user,
    I want to be able to filter course Q&As by topics, in order to
    easier help me find what I’m looking for.
    """;

    internal const string USER_DESIRABLE_6 = """
    AUTH-USER.DESIRABLE.6: As a user,
    I want to be able to format my questions (with headlines, italic,
    bold, …) and math formulas.
    """;

    internal const string USER_DESIRABLE_7 = """
    AUTH-USER.DESIRABLE.7: As a user, I want to be able to edit my
    questions and answers.
    """;

    internal const string USER_DESIRABLE_8 = """
    AUTH-USER.DESIRABLE.8: As a user who created a question,
    I want to mark an answer as especially helpful.
    """;

    internal const string USER_DESIRABLE_9 = """
    AUTH-USER.DESIRABLE.9: As a user,
    I want to be prevented from voting several times on an answer to
    prevent misuse by other users.
    """;

    #endregion User Story Desirable
}
