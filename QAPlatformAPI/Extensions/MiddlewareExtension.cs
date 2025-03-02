using QAPlatformAPI.Middlewares;

namespace QAPlatformAPI.Extensions;

public static class MiddlewareExtension
{
    public static void UseMiddlewareExtension(this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
        // app.UseMiddleware<ForwardedHeadersMiddleware>();
        app.UseMiddleware<RequestSizeLimitMiddleware>();
    }
}
