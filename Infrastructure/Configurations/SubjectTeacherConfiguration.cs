using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

/// <summary>
/// Custom config to create join table SubjectTeacher.
/// </summary>
public class SubjectTeacherConfiguration : IEntityTypeConfiguration<Subject>
{
    private readonly string _tableName = "SubjectTeacher";

    private readonly string _fkUserId = "UserId";

    private readonly string _fkSubjectId = "SubjectId";

    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.HasMany(subject => subject.Teachers)
            .WithMany(teacher => teacher.Subjects)
            .UsingEntity<Dictionary<string, object>>(
                _tableName,
                joinTable => joinTable.HasOne<User>().WithMany().HasForeignKey(_fkUserId),
                joinTable => joinTable.HasOne<Subject>().WithMany().HasForeignKey(_fkSubjectId)
            );
    }
}
