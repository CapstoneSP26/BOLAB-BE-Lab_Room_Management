using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        // Table name
        builder.ToTable("roles");

        // Primary key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .ValueGeneratedNever();

        // Role name
        builder.Property(x => x.RoleName)
               .IsRequired()
               .HasMaxLength(50);

        // Unique role name
        builder.HasIndex(x => x.RoleName)
               .IsUnique();
    }
}
