using Domain.Entities;
using Infrastructure.Contexts;
using Infrastructure.Seeds;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
        await SeedQAPlatformDBDevelopment.RunAsync(context, userManager, roleManager);
    }

    public static async Task UseProductionDataSeedExtension(this IApplicationBuilder app)
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
        await SeedQAPlatformDBProduction.RunAsync(context, userManager, roleManager);
    }
}
