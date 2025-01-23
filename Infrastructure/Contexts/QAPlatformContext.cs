using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

public class QAPlatformContext : IdentityDbContext<User, IdentityRole, string>
{
    public QAPlatformContext() { }

    public QAPlatformContext(DbContextOptions<QAPlatformContext> options)
        : base(options)
    { }
}
