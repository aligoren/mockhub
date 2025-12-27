using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MockHub.Domain.Entities;

namespace MockHub.Infrastructure.Data.Configurations;

public class TeamMemberConfiguration : IEntityTypeConfiguration<TeamMember>
{
    public void Configure(EntityTypeBuilder<TeamMember> builder)
    {
        builder.ToTable("TeamMembers");

        builder.HasKey(tm => tm.Id);

        builder.Property(tm => tm.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.HasIndex(tm => new { tm.TeamId, tm.UserId })
            .IsUnique();

        builder.HasIndex(tm => tm.UserId);
    }
}


