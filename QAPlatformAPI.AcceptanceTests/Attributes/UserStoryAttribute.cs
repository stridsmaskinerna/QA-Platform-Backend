namespace QAPlatformAPI.AcceptanceTests.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class UserStoryAttribute : FactAttribute
{
    public string Description { get; }

    public UserStoryAttribute(string description)
    {
        Description = description;
        DisplayName = $"{description}";
    }
}
