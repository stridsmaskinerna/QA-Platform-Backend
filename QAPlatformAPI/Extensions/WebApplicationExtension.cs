using Infrastructure.Contexts;
using Infrastructure.Seeds;

namespace QAPlatformAPI;

public static class WebApplicationExtension
{
    internal static async Task UseDataSeedAsyncExtension(this IApplicationBuilder applicationBuilder)
    {
        Console.WriteLine("Seeding data...");
        using var scope = applicationBuilder.ApplicationServices.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var context = serviceProvider.GetRequiredService<QAPlatformContext>();

        await SeedQAPlatformDB.RunAsync(context);
    }
}
