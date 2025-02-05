using Presentation;

namespace QAPlatformAPI.Extensions;

public static class ControllerExtension
{
    public static void AddControllerExtension(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers(configure =>
        {
            configure.ReturnHttpNotAcceptable = true;
        }).AddApplicationPart(typeof(PresentationAssembly).Assembly);
    }
}
