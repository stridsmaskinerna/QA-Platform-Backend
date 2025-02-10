using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Identity;

namespace QAPlatformAPI.Extensions;

public static class IdentityCoreExtension
{
    public static void AddIdentityCoreExtension(this IServiceCollection services)
    {
        services.AddIdentityCore<User>(opt =>
        {
            opt.Password.RequireDigit = false;
            opt.Password.RequireLowercase = false;
            opt.Password.RequireUppercase = false;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequiredLength = 8;

        })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<QAPlatformContext>()
            .AddDefaultTokenProviders();
    }
}
