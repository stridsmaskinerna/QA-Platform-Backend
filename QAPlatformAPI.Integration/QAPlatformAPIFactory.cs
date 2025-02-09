using Domain.Entities;
using Infrastructure.Contexts;
using Infrastructure.Seeds;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QAPlatformAPI.Extensions;

// Todo check if version 9 is of for DB in infra and startup project.
// Todo when running in github action docker database container must be started first.
public class QAPlatformAPIFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var testEnvironment = Environment.GetEnvironmentVariable("TEST_ENV");

        // Choose the correct appsettings file
        var settingsFileName = testEnvironment == "GITHUB_ACTIONS"
            ? "appsettings.Testing.CI.json"
            : "appsettings.Testing.Local.json";

        var testSettingsPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            settingsFileName);

        Console.WriteLine($"[DEBUG] Appsettings.Testing.json path: {testSettingsPath}");

        if (!File.Exists(testSettingsPath))
        {
            throw new FileNotFoundException($"Appsettings.Testing.json is missing: {testSettingsPath}");
        }

        var configuration = new ConfigurationBuilder()
            .AddJsonFile(testSettingsPath, optional: false, reloadOnChange: true)
            .Build();

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddConfiguration(configuration);
        });

        builder.ConfigureServices((context, services) =>
        {
            var connectionString = context.Configuration.GetConnectionString(
                "PostgreSQLConnection");
            Console.WriteLine($"[DEBUG] Loaded Connection String: {connectionString}");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string not found in appsettings.Testing.json.");
            }

            var serviceDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<QAPlatformContext>));
            if (serviceDescriptor != null)
            {
                services.Remove(serviceDescriptor);
            }

            services.AddDbContext<QAPlatformContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddControllerExtension();
            services.AddCORSConfigurationExtension(context.Configuration);
            services.AddIdentityCoreExtension();
            services.AddJSONSerializerOptionsExtension();
            services.AddApplicationServicesExtension();

            // Set up database before tests
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            SeedTestDatabase(scope).Wait();
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(IAuthenticationSchemeProvider));
            // services.RemoveAll(typeof(IAuthorizationPolicyProvider));
            services.RemoveAll(typeof(IConfigureOptions<AuthenticationOptions>));

            services.AddJWTSecurityExtension(configuration);
        });
    }

    private async Task SeedTestDatabase(IServiceScope scope)
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<QAPlatformContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // await dbContext.Database.EnsureDeletedAsync();
        // await dbContext.Database.MigrateAsync();
        // await SeedQAPlatformDBProduction.RunAsync(dbContext, userManager, roleManager);

        int retries = 5;
        while (retries > 0)
        {
            try
            {
                Console.WriteLine("Trying to connect to the database...");
                await dbContext.Database.EnsureDeletedAsync();
                await dbContext.Database.MigrateAsync();
                await SeedQAPlatformDBProduction.RunAsync(dbContext, userManager, roleManager);
                break;  // ✅ Exit loop if successful
            }
            catch (Exception ex)
            {
                retries--;
                Console.WriteLine($"Database connection failed: {ex.Message}. Retrying in 5s...");
                await Task.Delay(5000);  // Wait 5 seconds before retrying
            }
        }
    }
}
