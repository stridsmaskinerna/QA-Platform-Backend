using Domain.Entities;
using Infrastructure.Contexts;
using Infrastructure.Seeds;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace QAPlatformAPI;

public static class WebApplicationPipelineExtension
{
    internal static async Task UseDataSeedAsyncExtension(this IApplicationBuilder applicationBuilder)
    {
        using var scope = applicationBuilder.ApplicationServices.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var context = serviceProvider.GetRequiredService<QAPlatformContext>();
        await context.Database.MigrateAsync();
        if (await context.Subjects.AnyAsync() || await context.Users.AnyAsync())
        {
            return;
        }
        Console.WriteLine("Seeding data...");
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        await SeedQAPlatformDB.RunAsync(context, userManager, roleManager);
    }
}
