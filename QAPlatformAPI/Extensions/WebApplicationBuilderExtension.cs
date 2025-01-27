using System.Text.Json;
using Application.Services;
using Infrastructure.Contexts;
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
}
