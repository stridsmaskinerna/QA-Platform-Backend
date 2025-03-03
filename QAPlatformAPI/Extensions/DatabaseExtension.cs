using System.Runtime.ConstrainedExecution;
using Domain.Entities;
using Infrastructure.Contexts;
using Infrastructure.Seeds.Dev;
using Infrastructure.Seeds.Prod;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace QAPlatformAPI.Extensions;

public static class DatabaseExtension
{
    public static void AddDatabaseExtension(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment
    )
    {

        services.AddDbContext<QAPlatformContext>(options =>
        {
            var connectionString =
                configuration.GetConnectionString("PostgreSQLConnection")
                ?? throw new InvalidOperationException("Connection string not found.");

            options.UseNpgsql(connectionString);

            if (environment.IsDevelopment())
            {
                Console.WriteLine($"Connection String: {connectionString}");
                options.EnableSensitiveDataLogging();
            }
        });
    }

    public static async Task UseDevelopmentDataSeedExtension(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
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
        await DBSeedDev.RunAsync(context, userManager, roleManager);
    }

    public static async Task UseProductionDataSeedExtension(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var adminMail = configuration["SeedData:AdminMail"];
        var adminPassword = configuration["SeedData:AdminPassword"];
        var context = serviceProvider.GetRequiredService<QAPlatformContext>();
        await context.Database.MigrateAsync();
        if (await context.Subjects.AnyAsync() || await context.Users.AnyAsync())
        {
            return;
        }
        Console.WriteLine("Seeding data...");
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        await DBSeedProd.RunAsync(
            context, userManager, roleManager, adminMail, adminPassword);
    }
}
