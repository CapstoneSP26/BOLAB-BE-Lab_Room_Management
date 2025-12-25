using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations.Academic;

public class StudentGroupConfiguration : IEntityTypeConfiguration<StudentGroup>
{
    public void Configure(EntityTypeBuilder<StudentGroup> builder)
    {
        // Table
        builder.ToTable("student_groups");

        // Primary Key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .ValueGeneratedNever();

        // Group name
        builder.Property(x => x.GroupName)
               .IsRequired()
               .HasMaxLength(150);

        // Soft delete
        builder.Property(x => x.IsDeleted)
               .IsRequired()
               .HasDefaultValue(false);

        // Audit fields
        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.UpdatedAt)
               .IsRequired(false);

        builder.Property(x => x.CreatedBy)
               .IsRequired(false);

        builder.Property(x => x.UpdatedBy)
               .IsRequired(false);

        // Indexes
        builder.HasIndex(x => x.GroupName);
        builder.HasIndex(x => x.IsDeleted);
    }
}
