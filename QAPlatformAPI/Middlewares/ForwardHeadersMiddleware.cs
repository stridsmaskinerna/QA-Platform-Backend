//using Microsoft.AspNetCore.HttpOverrides;

//namespace QAPlatformAPI.Middlewares;

//public class ForwardedHeadersMiddleware(RequestDelegate next)
//{
//    public async Task InvokeAsync(HttpContext context)
//    {
//        var options = new ForwardedHeadersOptions
//        {
//            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
//        };

//        // Clearing KnownNetworks and KnownProxies to trust Cloudflare
//        options.KnownNetworks.Clear();
//        options.KnownProxies.Clear();

//        context.RequestServices.GetRequiredService<IApplicationBuilder>()
//            .UseForwardedHeaders(options);

//        await next(context);
//    }
//}
