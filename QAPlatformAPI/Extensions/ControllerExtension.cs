using Presentation;

namespace QAPlatformAPI.Extensions;

public static class ControllerExtension
{
    public static void AddControllerExtension(this IServiceCollection services)
    {
        services.AddControllers(configure =>
        {
            configure.ReturnHttpNotAcceptable = true;
        }).AddApplicationPart(typeof(PresentationAssembly).Assembly);
    }
}
