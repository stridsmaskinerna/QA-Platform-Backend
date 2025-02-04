using QAPlatformAPI.Filters;

namespace QAPlatformAPI.Extensions;

public static class OpenAPIExtension
{
    public static void AddOpenAPIExtension(this WebApplicationBuilder builder)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(opt =>
        {
            opt.OperationFilter<CustomHeadersOperationFilter>();
        });
    }

    public static void UseOpenAPIExtension(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
}
