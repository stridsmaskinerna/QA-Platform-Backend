using Domain.Constants;
using Microsoft.Extensions.Configuration;

namespace QAPlatformAPI.Extensions;

public static class CORSExtension
{
    private static readonly string CORS_DEV_POLICY = "cors_dev_policy";

    private static readonly string CORS_PROD_POLICY = "cors_prod_policy";

    public static void AddCORSConfigurationExtension(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment
    )
    {
        var allowedOrigins = configuration.GetSection("CORS:AllowedOrigins")
            .Get<string[]>() ?? Array.Empty<string>();

        Console.WriteLine($"Allowed origins [TEST_3]: {string.Join(", ", allowedOrigins)}");

        Console.WriteLine($"Allowed origins [TEST_2]: {allowedOrigins}");

        if (environment.IsProduction())
        {
            services.AddCors(config =>
            {
                config.AddPolicy(CORS_PROD_POLICY, p => p
                    .WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader() // Allows `Authorization` header
                    .AllowCredentials()
                    .WithExposedHeaders([
                        CustomHeaders.Pagination
                    ])
                );
            });
        }
        else
        {
            services.AddCors(config =>
            {
                config.AddPolicy(CORS_DEV_POLICY, p => p
                    .WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader() // Allows `Authorization` header
                    .WithExposedHeaders([
                        CustomHeaders.Pagination
                    ])
                );
            });
        }
    }

    public static void UseCORSDevelopmentPolicyExtension(this WebApplication app)
    {
        app.UseCors(CORS_DEV_POLICY);
    }

    public static void UseCORSProductionPolicyExtension(this WebApplication app)
    {
        app.UseCors(CORS_PROD_POLICY);
    }
}
