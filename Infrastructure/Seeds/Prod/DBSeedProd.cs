using Bogus;
using Domain.Constants;
using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Seeds.Prod;

public static class DBSeedProd
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
        string password = SeedConstantsProd.DEFAULT_PWD
    )
    {
        var admin = new User()
        {
            UserName = SeedConstantsProd.ADMIN_USERNAME,
            Email = SeedConstantsProd.ADMIN_EMAIL
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
