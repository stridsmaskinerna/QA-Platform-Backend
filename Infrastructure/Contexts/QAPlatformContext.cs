using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

public class QAPlatformContext : Microsoft.EntityFrameworkCore.DbContext
{
    public QAPlatformContext() { }

    public QAPlatformContext(DbContextOptions<QAPlatformContext> options)
        : base(options)
    { }
}
