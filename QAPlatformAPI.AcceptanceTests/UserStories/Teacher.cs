using QAPlatformAPI.AcceptanceTests.Attributes;
using QAPlatformAPI.IntegrationTests;

namespace QAPlatformAPI.AcceptanceTests.UserStories;

[Collection("Sequential")]
public class Teacher : UserStoryTestBase
{
    public Teacher(QAPlatformAPIFactory<Program> factory) :
        base(factory)
    { }

    public class Essential : Teacher
    {
        public Essential(QAPlatformAPIFactory<Program> factory) :
            base(factory)
        { }

        [UserStory(Description.TEACHER_ESSE_1, Skip = "Not tested")]
        public async Task TEACHER_ESSE_1()
        {

        }

        [UserStory(Description.TEACHER_ESSE_3, Skip = "Not tested")]
        public async Task TEACHER_ESSE_3()
        {

        }

        [UserStory(Description.TEACHER_ESSE_4, Skip = "Not implemented and tested")]
        public async Task TEACHER_ESSE_4()
        {

        }
    }

    public class Desirable : Teacher
    {
        public Desirable(QAPlatformAPIFactory<Program> factory) :
            base(factory)
        { }

        [UserStory(Description.TEACHER_DESIRABLE_1, Skip = "Not implemented and tested")]
        public async Task TEACHER_DESIRABLE_1()
        {

        }
    }
}

file class Description
{
    #region User Stories Essential

    internal const string TEACHER_ESSE_1 = """
    TEACHER.ESSE.1: As a teacher,
    I want my answer to be highlighted so that my answers can
    be more easily identified.
    """;

    internal const string TEACHER_ESSE_3 = """
    TEACHER.ESSE.3: As a teacher,
    I should be able to create, modify, and delete topics which
    the questions can be associated with, this makes relevant
    questions easier to find
    """;

    internal const string TEACHER_ESSE_4 = """
    TEACHER.ESSE.4: As a teacher,
    I want to be able to hide answers and questions in my course
    """;

    #endregion

    #region User Stories Desirable

    internal const string TEACHER_DESIRABLE_1 = """
    TEACHER.DESIRABLE.1: As a teacher,
    I want to quickly find the questions related to my courses,
    i.e. not by having to type with the keyboard.
    """;

    #endregion User Stories Desirable

}
