using System.Text.Json;

namespace QAPlatformAPI.Extensions;

public static class JSONSerializerExtension
{
    public static void AddJSONSerializerOptionsExtension(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });
    }
}
