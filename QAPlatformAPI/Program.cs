using QAPlatformAPI.Extensions;

namespace QAPlatformAPI;

public class Program
{
    public static async Task Main(string[] args)
    {
        var app = CreateWebApplication(args);
        await ConfigureWebApplicationPipeline(app);
    }

    private static WebApplication CreateWebApplication(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Console.WriteLine($"ENVIRONMENT: {builder.Environment.EnvironmentName}");

        // var allowedOrigins = builder.Configuration.GetSection("CORS:AllowedOrigins").Get<string[]>();

        builder.Services.AddDatabaseExtension(builder.Configuration, builder.Environment);

        builder.Services.AddControllerExtension();

        builder.Services.AddCORSConfigurationExtension(builder.Configuration, builder.Environment);

        builder.Services.AddJWTSecurityExtension(builder.Configuration);

        builder.Services.AddIdentityCoreExtension();

        builder.Services.AddJSONSerializerOptionsExtension();

        builder.Services.AddApplicationServicesExtension();

        builder.Services.AddOpenAPIExtension();

        return builder.Build();
    }

    private static async Task ConfigureWebApplicationPipeline(WebApplication app)
    {
        app.UseMiddlewareExtension();

        if (app.Environment.IsDevelopment())
        {
            app.UseOpenAPIExtension();
            await app.UseDevelopmentDataSeedExtension();
            app.UseCORSDevelopmentPolicyExtension();
        }
        else
        {
            await app.UseProductionDataSeedExtension();
            app.UseCORSProductionPolicyExtension();
        }

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
