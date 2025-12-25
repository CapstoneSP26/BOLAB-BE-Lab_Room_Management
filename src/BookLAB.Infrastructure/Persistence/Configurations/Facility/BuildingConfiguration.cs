using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations.Facility;

public class BuildingConfiguration : IEntityTypeConfiguration<Building>
{
    public void Configure(EntityTypeBuilder<Building> builder)
    {
        builder.ToTable("buildings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .ValueGeneratedNever();

        builder.Property(x => x.BuildingName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.NumberOfFloors)
               .IsRequired();

        builder.HasIndex(x => new { x.CampusId, x.BuildingName })
               .IsUnique();

        builder.HasOne<Campus>()
               .WithMany()
               .HasForeignKey(x => x.CampusId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
