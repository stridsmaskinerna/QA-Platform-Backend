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
}
