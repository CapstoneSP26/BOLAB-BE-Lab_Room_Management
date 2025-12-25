using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations.Identity;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Table name (plural, snake_case is optional – PostgreSQL friendly)
        builder.ToTable("users");
        // Primary Key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .ValueGeneratedNever();

        // Email
        builder.Property(x => x.Email)
               .IsRequired()
               .HasMaxLength(255);

        builder.HasIndex(x => x.Email)
               .IsUnique();

        // Full name
        builder.Property(x => x.FullName)
               .IsRequired()
               .HasMaxLength(150);

        // Student ID (nullable, but unique when exists)
        builder.Property(x => x.StudentId)
               .HasMaxLength(50);

        builder.HasIndex(x => x.StudentId)
               .IsUnique();
        // PostgreSQL partial index

        // Status flags
        builder.Property(x => x.IsActive)
               .HasDefaultValue(true);

        builder.Property(x => x.IsDeleted)
               .HasDefaultValue(false);

        // Soft delete global filter
        builder.HasQueryFilter(x => !x.IsDeleted);

        // Relationships
        builder.HasMany(x => x.UserRoles)
               .WithOne()
               .HasForeignKey(ur => ur.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
