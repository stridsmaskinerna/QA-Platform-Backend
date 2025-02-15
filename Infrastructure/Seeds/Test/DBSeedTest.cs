using Bogus;
using Domain.Constants;
using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Seeds.Test;

public static class DBSeedTest
{
    private static readonly IBaseSeeder _seeder = new BaseSeeder();

    public static async Task RunAsync(
        QAPlatformContext context,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager
    )
    {
        var subjects = CreateSubjects();
        await context.AddRangeAsync(subjects);

        await _seeder.CreateUserRoles(roleManager);

        var topics = _seeder.CreateTopics(subjects);
        await context.AddRangeAsync(topics);

        var tags = _seeder.CreateTags();
        await context.AddRangeAsync(tags);

        var users = await CreateUsers(subjects, userManager);

        var questions = _seeder.CreateQuestions(10, topics, tags, users);
        await context.AddRangeAsync(questions);

        await context.SaveChangesAsync();
    }

    private static async Task<List<User>> CreateUsers(
        List<Subject> subjects,
        UserManager<User> userManager
    )
    {
        return [
            await CreateTestAdmin(userManager),
            await CreateTestTeacher(subjects, userManager),
            await CreateTestUser(userManager)
        ];
    }

    private static List<Subject> CreateSubjects()
    {
        var subjects = new HashSet<Subject>();

        var generalSubject = new Faker<Subject>().Rules((f, s) =>
        {
            s.Name = SeedConstant.general;
            s.SubjectCode = null;
        });
        subjects.Add(generalSubject);

        var testSubject = new Faker<Subject>().Rules((f, s) =>
        {
            s.Name = SeedConstantsTest.A_SEEDED_TEST_SUBJECT;
            s.SubjectCode = "TEST_SUBJECT_ID";
        });
        subjects.Add(testSubject);

        return [.. subjects];
    }

    private static async Task<User> CreateTestAdmin(
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
        if (!result.Succeeded) throw new SeedException(string.Join("\n", result.Errors));
        await userManager.AddToRoleAsync(testAdmin, DomainRoles.USER);
        await userManager.AddToRoleAsync(testAdmin, DomainRoles.TEACHER);
        await userManager.AddToRoleAsync(testAdmin, DomainRoles.ADMIN);

        return testAdmin;
    }

    private static async Task<User> CreateTestTeacher(
        List<Subject> subjects,
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

        for (int j = 0; j < subjects.Count; j++)
        {
            if (subjects[j].Name == SeedConstant.general)
            {
                continue;
            }

            testTeacher.Subjects.Add(subjects[j]);
        }

        return testTeacher;
    }

    private static async Task<User> CreateTestUser(
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

        return testUser;
    }
}
