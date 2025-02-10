using Bogus;
using Domain.Constants;
using Domain.Entities;
using Infrastructure.Contexts;
using Infrastructure.Seeds.Prod;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Seeds.Test;

public static class DBSeedTest
{
    public static async Task RunAsync(
        QAPlatformContext context,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager
    )
    {
        var subjects = CreateSubjects();
        await context.AddRangeAsync(subjects);

        await CreateUserRoles(roleManager);

        await CreateTestAdmin(userManager);

        await CreateTestTeacher(userManager);

        await CreateTestUser(userManager);

        var topics = CreateTopics(subjects);
        await context.AddRangeAsync(topics);

        await context.SaveChangesAsync();
    }

    private static List<Subject> CreateSubjects()
    {
        var subjects = new HashSet<Subject>();

        var generalSubject = new Faker<Subject>().Rules((f, s) =>
        {
            s.Name = $"General";
            s.SubjectCode = null;
        });
        subjects.Add(generalSubject);

        return [.. subjects];
    }

    internal static async Task CreateUserRoles(
        RoleManager<IdentityRole> roleManager
    )
    {
        await DBSeedProd.CreateUserRoles(roleManager);
    }

    private static async Task CreateTestAdmin(
        UserManager<User> userManager,
        string password = SeedConstantsTest.DEFAULT_PWD
    )
    {
        var testAdmin = new User()
        {
            UserName = SeedConstantsTest.ADMIN_USERNAME,
            Email = SeedConstantsTest.ADMIN_EMAIL
        };

        var result = await userManager.CreateAsync(testAdmin, password);
        if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));
        await userManager.AddToRoleAsync(testAdmin, DomainRoles.USER);
        await userManager.AddToRoleAsync(testAdmin, DomainRoles.TEACHER);
        await userManager.AddToRoleAsync(testAdmin, DomainRoles.ADMIN);
    }

    private static async Task CreateTestTeacher(
        UserManager<User> userManager,
        string password = SeedConstantsTest.DEFAULT_PWD
    )
    {

        var testTeacher = new User()
        {
            UserName = SeedConstantsTest.TEACHER_EMAIL,
            Email = SeedConstantsTest.TEACHER_EMAIL
        };

        var result = await userManager.CreateAsync(testTeacher, password);
        if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));
        await userManager.AddToRoleAsync(testTeacher, DomainRoles.USER);
        await userManager.AddToRoleAsync(testTeacher, DomainRoles.TEACHER);
    }

    private static async Task CreateTestUser(
        UserManager<User> userManager,
        string password = SeedConstantsTest.DEFAULT_PWD
    )
    {
        var testUser = new User()
        {
            UserName = SeedConstantsTest.USER_USERNAME,
            Email = SeedConstantsTest.USER_EMAIL
        };

        var result = await userManager.CreateAsync(testUser, password);
        if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));
        await userManager.AddToRoleAsync(testUser, DomainRoles.USER);
    }

    private static List<Topic> CreateTopics(
        List<Subject> subjects
    )
    {
        var topics = new List<Topic>();

        foreach (var subject in subjects)
        {
            var nameOfDefaultSubject = subject.Name;

            var topic = new Topic()
            {
                Subject = subject,
                SubjectId = subject.Id,
                Name = nameOfDefaultSubject,
                IsActive = true
            };
            topics.Add(topic);
        }

        return topics;
    }
}
