using Bogus;
using Domain.Constants;
using Domain.Entities;
using Infrastructure.Contexts;
using Infrastructure.Seeds.Base;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Seeds.Prod;

public static class DBSeedProd
{
    private static readonly IBaseSeeder _seeder = new BaseSeeder();

    public static async Task RunAsync(
        QAPlatformContext context,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        string? adminMail,
        string? adminPassword
    )
    {
        var subjects = CreateSubjects();
        await context.AddRangeAsync(subjects);

        await _seeder.CreateUserRoles(roleManager);

        await CreateUsers(
            userManager,
            adminMail ?? SeedDataProd.ADMIN_EMAIL,
            adminPassword ?? SeedDataProd.DEFAULT_PWD);

        var topics = CreateTopics(subjects);
        await context.AddRangeAsync(topics);

        await context.SaveChangesAsync();
    }

    private static List<Subject> CreateSubjects()
    {
        var subjects = new HashSet<Subject>();

        var generalSubject = new Faker<Subject>().Rules((f, s) =>
        {
            s.Name = SeedDataProd.generalSubject;
            s.SubjectCode = null;
        });
        subjects.Add(generalSubject);

        return [.. subjects];
    }

    private static async Task CreateUsers(
        UserManager<User> userManager,
        string adminMail,
        string adminPassword
    )
    {
        Console.WriteLine($"Admin pwd: {adminPassword}");

        var admin = new User()
        {
            UserName = SeedDataProd.ADMIN_USERNAME,
            Email = adminMail
        };

        var result = await userManager.CreateAsync(admin, adminPassword);
        if (!result.Succeeded) throw new SeedException(string.Join("\n", result.Errors));
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
