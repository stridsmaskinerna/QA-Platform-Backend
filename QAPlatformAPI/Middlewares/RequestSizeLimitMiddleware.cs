namespace QAPlatformAPI.Middlewares;

public class RequestSizeLimitMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var maxRequestSize = 2 * 1024 * 1024; // 2MB limit
        if (context.Request.ContentLength > maxRequestSize)
        {
            context.Response.StatusCode = StatusCodes.Status413PayloadTooLarge;
            await context.Response.WriteAsync("Request body too large.");
            return;
        }

        await next(context);
    }
}
