using Domain;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Presentation;
using QAPlatformAPI.Extensions;
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

        builder.Services.AddControllers(configure =>
        {
            configure.ReturnHttpNotAcceptable = true;
        }).AddApplicationPart(typeof(AssemblyReference).Assembly);

        builder.AddCORSConfiguration();

        builder.AddDBExtension();

        builder.AddIdentityCoreExtension();

        builder.AddJSONSerializerOptions();

        builder.AddApplicationServices();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddAutoMapper(typeof(MapperManager));
        builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();

        return builder.Build();
    }

    private static async Task ConfigureWebApplicationPipeline(WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            // await app.UseDataSeedAsyncExtension();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors("AllowFrontend");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
