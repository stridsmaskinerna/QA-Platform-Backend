using Presentation;
using QAPlatformAPI.Extensions;
using QAPlatformAPI.Filters;
using QAPlatformAPI.Middlewares;

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

        builder.AddControllerExtension();

        builder.AddCORSConfigurationExtension();

        builder.AddDatabaseExtension();

        builder.AddJWTSecurityExtension();

        builder.AddIdentityCoreExtension();

        builder.AddJSONSerializerOptionsExtension();

        builder.AddApplicationServicesExtension();

        builder.AddOpenAPIExtension();

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
