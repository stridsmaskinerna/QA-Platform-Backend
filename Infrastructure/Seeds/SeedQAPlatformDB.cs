using Infrastructure.Contexts;

namespace Infrastructure.Seeds;

public static class SeedQAPlatformDB
{
    public static async Task RunAsync(
    QAPlatformContext context)
    {
        await context.SaveChangesAsync();
    }
}
