using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations.Facility;

public class CampusConfiguration : IEntityTypeConfiguration<Campus>
{
    public void Configure(EntityTypeBuilder<Campus> builder)
    {
        builder.ToTable("campuses");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .ValueGeneratedNever();

        builder.Property(x => x.CampusName)
               .IsRequired()
               .HasMaxLength(100);

        builder.HasIndex(x => x.CampusName)
               .IsUnique();

        builder.Property(x => x.Address)
               .HasMaxLength(255);

        builder.Property(x => x.IsActive)
               .HasDefaultValue(true);
    }
}
