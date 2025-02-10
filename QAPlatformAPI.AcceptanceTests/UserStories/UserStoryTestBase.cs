using System.Reflection;
using QAPlatformAPI.IntegrationTests;

namespace QAPlatformAPI.AcceptanceTests.UserStories;

public abstract class UserStoryTestBase : IntegrationTestBase
{
    protected UserStoryTestBase(
        QAPlatformAPIFactory<Program> factory) : base(factory)
    { }

    /// <summary>
    /// Used to call test methods dynamically from test classes.
    /// Do only support test methods with facts not theories.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    protected async Task RunTestsAsync<T>() where T : class
    {
        var testMethods = typeof(T)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.GetCustomAttributes(typeof(FactAttribute), false).Any())
            .ToList();

        if (testMethods.Count == 0)
        {
            throw new InvalidOperationException($"No test methods found in {typeof(T).Name}");
        }

        var testInstance = Activator.CreateInstance(
            typeof(T),
            _factory);

        foreach (var method in testMethods)
        {
            if (method.Invoke(testInstance, null) is Task task)
            {
                await task;
            }
        }
    }
}
