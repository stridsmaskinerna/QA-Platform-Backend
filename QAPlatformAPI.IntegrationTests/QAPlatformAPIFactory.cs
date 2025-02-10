using Domain.Entities;
using Infrastructure.Contexts;
using Infrastructure.Seeds.Test;
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

namespace QAPlatformAPI.IntegrationTests;

public class QAPlatformAPIFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var testEnvironment = Environment.GetEnvironmentVariable("TEST_ENV");

        var settingsFileName = "appsettings.Testing.json";

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
            SeedTestDatabase(scope, connectionString).Wait();
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(IAuthenticationSchemeProvider));
            // services.RemoveAll(typeof(IAuthorizationPolicyProvider));
            services.RemoveAll(typeof(IConfigureOptions<AuthenticationOptions>));

            services.AddJWTSecurityExtension(configuration);
        });
    }

    private async Task SeedTestDatabase(
        IServiceScope scope,
        string connectionString
    )
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<QAPlatformContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        int retries = 5;
        while (retries > 0)
        {
            try
            {
                Console.WriteLine("Trying to connect to the database...");
                Console.WriteLine($"[DEBUG] Loaded Connection String: {connectionString}");

                await dbContext.Database.EnsureDeletedAsync();
                await dbContext.Database.MigrateAsync();
                await DBSeedTest.RunAsync(dbContext, userManager, roleManager);
                break;
            }
            catch (Exception ex)
            {
                retries--;
                Console.WriteLine($"Database connection failed: {ex.Message}. Retrying in 5s...");
                Console.WriteLine($"[DEBUG] Loaded Connection String: {connectionString}");

                await Task.Delay(5000);
            }
        }
    }
}
