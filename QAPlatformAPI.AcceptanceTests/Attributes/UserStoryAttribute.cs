namespace QAPlatformAPI.AcceptanceTests.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class UserStoryAttribute : FactAttribute
{
    public string UserStory { get; }

    public UserStoryAttribute(string userStory)
    {
        UserStory = userStory;
        DisplayName = $"{userStory}";
    }
}
