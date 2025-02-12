namespace QAPlatformAPI.Extensions;

public static class OpenAPIExtension
{
    public static void AddOpenAPIExtension(this IServiceCollection services)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.EnableAnnotations();
        });
    }

    public static void UseOpenAPIExtension(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
}
