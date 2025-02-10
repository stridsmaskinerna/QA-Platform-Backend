using System.Text.Json;

namespace QAPlatformAPI.Extensions;

public static class JSONSerializerExtension
{
    public static void AddJSONSerializerOptionsExtension(this IServiceCollection services)
    {
        services.AddSingleton(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });
    }
}
