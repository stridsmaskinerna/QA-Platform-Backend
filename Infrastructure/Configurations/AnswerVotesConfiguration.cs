using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

/// <summary>
/// Custom config to create join table SubjectTeacher.
/// </summary>
public class AnswerVotesConfiguration : IEntityTypeConfiguration<AnswerVotes>
{


    public void Configure(EntityTypeBuilder<AnswerVotes> builder)
    {
        builder.HasKey(av => new { av.UserId, av.AnswerId });
    }
}
