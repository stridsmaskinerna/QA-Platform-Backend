using Domain.Entities;
using Infrastructure.Contexts;
using Infrastructure.Seeds;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using QAPlatformAPI.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Domain.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;

namespace QAPlatformAPI.IntegrationTests;

public class QAPlatformAPIFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        // ✅ Load `appsettings.Testing.json` from the test project's root directory
        var testSettingsPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "appsettings.Testing.json");

        if (!File.Exists(testSettingsPath))
        {
            throw new FileNotFoundException($"Could not find appsettings.Testing.json at {testSettingsPath}");
        }

        // ✅ Manually create a new WebApplicationBuilder to call extensions
        var testBuilder = WebApplication.CreateBuilder();

        // ✅ Call your extensions that expect `WebApplicationBuilder`
        testBuilder.AddControllerExtension();
        testBuilder.AddCORSConfigurationExtension();
        testBuilder.AddIdentityCoreExtension();
        testBuilder.AddJWTSecurityExtension();
        testBuilder.AddJSONSerializerOptionsExtension();
        testBuilder.AddApplicationServicesExtension();

        builder.ConfigureServices(services =>
        {
            // ✅ Remove existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<QAPlatformContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            var connectionString =
                testBuilder.Configuration.GetConnectionString("PostgreSQLConnection")
                ?? throw new InvalidOperationException("Connection string not found.");

            // ✅ Register test database in builder (not in testBuilder)
            services.AddDbContext<QAPlatformContext>(options =>
                options.UseNpgsql(connectionString));

            // ✅ Copy all services from `testBuilder` to `builder`
            foreach (var service in testBuilder.Services)
            {
                services.Add(service);
            }

            // ✅ Ensure database is set up before tests
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            EnsureTestDatabaseSetup(scope).Wait();
        });

        builder.ConfigureTestServices(services =>
        {
            // ✅ Remove real authentication handlers
            var authHandler = services.FirstOrDefault(s => s.ServiceType == typeof(IAuthenticationSchemeProvider));
            if (authHandler != null) services.Remove(authHandler);

            // ✅ Remove real authentication handlers
            var policyProvider = services.FirstOrDefault(s => s.ServiceType == typeof(IAuthorizationPolicyProvider));
            if (policyProvider != null) services.Remove(policyProvider);

            // ✅ Add fake authentication for testing
            services.AddAuthentication("TestAuthScheme")
                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("TestAuthScheme", options => { });

            // ✅ Configure authorization to allow all authenticated users
            services.AddAuthorization(options =>
            {
                options.AddPolicy("TestPolicy", policy =>
                    policy.RequireAuthenticatedUser().RequireRole(DomainRoles.USER));
            });
        });
    }

    private async Task EnsureTestDatabaseSetup(IServiceScope scope)
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<QAPlatformContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // ✅ Ensure database is clean before each test
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.MigrateAsync();

        // ✅ Seed initial data
        await SeedQAPlatformDBProduction.RunAsync(dbContext, userManager, roleManager);
    }
}

/// <summary>
/// ✅ Custom test authentication handler (automatically authenticates requests)
/// </summary>
public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder
    ) : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] {
            new Claim(ClaimTypes.Name, "TestUser"),
            new Claim(ClaimTypes.Role, DomainRoles.USER)
        };
        var identity = new ClaimsIdentity(claims, "TestAuthScheme");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "TestAuthScheme");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

/// <summary>
/// ✅ Custom authorization handler that allows all requests for testing
/// </summary>
public class AllowAnonymousHandler : IAuthorizationHandler
{
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        foreach (var requirement in context.PendingRequirements.ToList())
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
