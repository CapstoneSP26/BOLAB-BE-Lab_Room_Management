using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations.Academic;

public class GroupMemberConfiguration : IEntityTypeConfiguration<GroupMember>
{
    public void Configure(EntityTypeBuilder<GroupMember> builder)
    {
        // Table
        builder.ToTable("group_members");

        // Primary key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .ValueGeneratedNever();

        // Required fields
        builder.Property(x => x.GroupId)
               .IsRequired();

        builder.Property(x => x.UserId)
               .IsRequired();

        // Indexes
        builder.HasIndex(x => x.GroupId);
        builder.HasIndex(x => x.UserId);

        // Prevent duplicate membership
        builder.HasIndex(x => new { x.GroupId, x.UserId })
               .IsUnique();

        // Relationships
        builder.HasOne<StudentGroup>()
               .WithMany()
               .HasForeignKey(x => x.GroupId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
