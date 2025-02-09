using System.Security.Claims;
using System.Text.Encodings.Web;
using Domain.Constants;
using Domain.Entities;
using Infrastructure.Contexts;
using Infrastructure.Seeds;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
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

public class QAPlatformAPIFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // TODO. Seems like production environment are used even if
        // explicit saying testing here
        builder.UseEnvironment("Testing");

        var testSettingsPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "appsettings.Testing.json");

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
            var connectionString = context.Configuration.GetConnectionString("PostgreSQLConnection");
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

        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.MigrateAsync();
        await SeedQAPlatformDBProduction.RunAsync(dbContext, userManager, roleManager);
    }
}
