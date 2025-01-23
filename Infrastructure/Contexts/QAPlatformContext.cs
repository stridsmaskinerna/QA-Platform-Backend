using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Configurations;

namespace Infrastructure.Contexts;

public class QAPlatformContext : IdentityDbContext<User, IdentityRole, string>
{
    public DbSet<Subject> Subjects => Set<Subject>();
    
    public DbSet<Topic> Topics => Set<Topic>();

    public DbSet<Question> Questions => Set<Question>();
    
    public DbSet<Comment> Comments => Set<Comment>();

    public DbSet<Answer> Answers => Set<Answer>();

    public QAPlatformContext() { }

    public QAPlatformContext(DbContextOptions<QAPlatformContext> options)
        : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new SubjectTeacherConfiguration());
    }
}
