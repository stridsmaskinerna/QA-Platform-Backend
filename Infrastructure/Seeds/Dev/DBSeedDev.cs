using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Seeds.Dev;

public static class DBSeedDev
{
    private static readonly IBaseSeeder _seeder = new BaseSeeder();

    public static async Task RunAsync(
        QAPlatformContext context,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager
    )
    {
        var subjects = _seeder.CreateSubjects();
        await context.AddRangeAsync(subjects);

        await _seeder.CreateUserRoles(roleManager);

        var users = await _seeder.CreateUsers(100, userManager, subjects);

        var topics = _seeder.CreateTopics(subjects);
        await context.AddRangeAsync(topics);

        var tags = _seeder.CreateTags();
        await context.AddRangeAsync(tags);

        var questions = _seeder.CreateQuestions(10, topics, tags, users);
        await context.AddRangeAsync(questions);

        var answers = _seeder.CreateAnswers(5, questions, users);
        await context.AddRangeAsync(answers);

        var comments = _seeder.CreateComments(5, answers, users);
        await context.AddRangeAsync(comments);

        var answerVotes = _seeder.CreateAnswerVotes(users, answers);
        await context.AddRangeAsync(answerVotes);

        await context.SaveChangesAsync();
    }
}
