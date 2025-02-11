using System.Reflection;
using QAPlatformAPI.IntegrationTests;

namespace QAPlatformAPI.AcceptanceTests.UserStories;

/// <summary>
/// Use class method RunTestsAsync to invoke
/// integration tests dynamically from acceptance tests.
/// </summary>
public abstract class UserStoryTestBase : IntegrationTestBase
{
    protected UserStoryTestBase(QAPlatformAPIFactory<Program> factory) :
        base(factory)
    { }

    /// <summary>
    /// Used to call test methods dynamically from test classes.
    /// Do support test methods with attributes
    ///     1) Facts
    ///     2) Theories with inlinedata attributes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>

    protected async Task RunTestsAsync<T>() where T : class
    {
        // Find all public methods with [Fact] or [Theory] in the specified test class
        var testMethods = typeof(T)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m =>
                m.GetCustomAttributes(typeof(FactAttribute), false).Length != 0 ||
                m.GetCustomAttributes(typeof(TheoryAttribute), false).Length != 0
            )
            .ToList();

        if (testMethods.Count == 0)
        {
            throw new InvalidOperationException($"No test methods found in {typeof(T).Name}");
        }

        // Create an instance of the test class
        var testInstance = Activator.CreateInstance(typeof(T), _factory);

        foreach (var method in testMethods)
        {
            var factAttribute = method.GetCustomAttribute<FactAttribute>();
            var theoryAttribute = method.GetCustomAttribute<TheoryAttribute>();

            // Check if test is marked as Skipped
            if ((factAttribute != null && !string.IsNullOrEmpty(factAttribute.Skip)) ||
                (theoryAttribute != null && !string.IsNullOrEmpty(theoryAttribute.Skip)))
            {
                Console.WriteLine($"Skipping test: {method.Name} - Reason: {factAttribute?.Skip ?? theoryAttribute?.Skip}");
                continue;
            }

            var theoryAttributes = method.GetCustomAttributes(typeof(TheoryAttribute), false);
            var inlineDataAttributes = method.GetCustomAttributes(typeof(InlineDataAttribute), false);

            // Handle and Invoke Theories with InlineData attributes
            if (inlineDataAttributes.Length != 0)
            {
                foreach (InlineDataAttribute inlineData in inlineDataAttributes)
                {
                    var parameters = inlineData.GetType()
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => p.Name == "Data")
                        .Select(p => p.GetValue(inlineData))
                        .FirstOrDefault() as object[];

                    if (parameters != null)
                    {
                        if (method.Invoke(testInstance, parameters) is Task task)
                        {
                            await task;
                        }
                    }
                }
            }
            // Handle and Invoke Test with Fact attribute
            else if (theoryAttributes.Length == 0)
            {
                if (method.Invoke(testInstance, null) is Task task)
                {
                    await task;
                }
            }
        }
    }
}

