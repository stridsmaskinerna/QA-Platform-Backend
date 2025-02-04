using Bogus;
using Domain.Constants;
using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Seeds;

public static class SeedQAPlatformDBProduction
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

        await CreateUsers(userManager);

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
        string[] roles =
        [
            DomainRoles.TEACHER,
            DomainRoles.USER,
            DomainRoles.ADMIN
        ];

        foreach (var roleName in roles)
        {
            if (await roleManager.RoleExistsAsync(roleName)) continue;
            var role = new IdentityRole { Name = roleName };
            var result = await roleManager.CreateAsync(role);

            if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));
        }
    }

    private static async Task CreateUsers(
        UserManager<User> userManager,
        string password = "adminPassword"
    )
    {
        var admin = new User()
        {
            UserName = "adminUser",
            Email = "admin@ltu.se"
        };

        var result = await userManager.CreateAsync(admin, password);
        if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));
        await userManager.AddToRoleAsync(admin, DomainRoles.USER);
        await userManager.AddToRoleAsync(admin, DomainRoles.TEACHER);
        await userManager.AddToRoleAsync(admin, DomainRoles.ADMIN);
    }

    private static List<Topic> CreateTopics(
        List<Subject> subjects
    )
    {
        var topics = new List<Topic>();
        var topicsNames = new[]
        {
            "Introduction"
        };

        foreach (var subject in subjects)
        {
            foreach (var name in topicsNames)
            {
                var topic = new Topic()
                {
                    Subject = subject,
                    SubjectId = subject.Id,
                    Name = name,
                    IsActive = true
                };
                topics.Add(topic);
            }
        }

        return topics;
    }
}
