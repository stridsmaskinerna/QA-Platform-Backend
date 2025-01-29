using System.Text.Json;
using Application.Services;
using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace QAPlatformAPI.Extensions;

public static class WebApplicationBuilderExtension
{
    public static void AddDBExtension(this WebApplicationBuilder builder)
    {

        builder.Services.AddDbContext<QAPlatformContext>(options =>
        {
            var connectionString =
                builder.Configuration.GetConnectionString("PostgreSQLConnection")
                ?? throw new InvalidOperationException("Connection string not found.");

            options.UseNpgsql(connectionString);

            Console.WriteLine($"Connection String: {connectionString}");

            if (builder.Environment.IsDevelopment())
            {
                Console.WriteLine($"Connection String: {connectionString}");
            }

            if (builder.Environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
            }
        });
    }

    public static void AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IServiceManager, ServiceManager>();
        builder.Services.AddAsLazy<IBaseService, BaseService>();
        builder.Services.AddAsLazy<IQuestionService, QuestionService>();
    }

    private static void AddAsLazy<IServiceType, ServiceType>(
        this IServiceCollection collection,
        ServiceLifetime lifetime = ServiceLifetime.Scoped
    )
        where ServiceType : class, IServiceType
        where IServiceType : class
    {
        collection.Add(new ServiceDescriptor(
            typeof(IServiceType),
            typeof(ServiceType),
            lifetime
        ));

        collection.Add(new ServiceDescriptor(
            typeof(Lazy<IServiceType>),
            p => new Lazy<IServiceType>(() => p.GetRequiredService<IServiceType>()),
            lifetime
        ));
    }

    public static void AddJSONSerializerOptions(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });
    }

    public static void AddIdentityCoreExtension(this WebApplicationBuilder builder)
    {
        builder.Services.AddDataProtection();

        builder.Services.AddIdentityCore<User>(opt =>
        {
            opt.Password.RequireDigit = false;
            opt.Password.RequireLowercase = false;
            opt.Password.RequireUppercase = false;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequiredLength = 8;

        })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<QAPlatformContext>()
            .AddDefaultTokenProviders();
    }

    public static void AddCORSConfiguration(this WebApplicationBuilder builder)
    {
        var allowedOrigins = builder.Configuration.GetSection("CORS:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

        builder.Services.AddCors(config =>
            config.AddPolicy("AllowFrontend",
                p => p.WithOrigins(allowedOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader()  // ✅ Allows `Authorization` header
            ));
    }
}
