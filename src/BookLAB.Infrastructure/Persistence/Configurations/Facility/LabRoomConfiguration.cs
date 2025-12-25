using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations.Facility;

public class LabRoomConfiguration : IEntityTypeConfiguration<LabRoom>
{
    public void Configure(EntityTypeBuilder<LabRoom> builder)
    {
        builder.ToTable("lab_rooms");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .ValueGeneratedNever();

        builder.Property(x => x.RoomName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.Location)
               .HasMaxLength(100);

        builder.Property(x => x.Capacity)
               .IsRequired();

        builder.Property(x => x.IsSpecialized)
               .HasDefaultValue(false);

        builder.Property(x => x.IsActive)
               .HasDefaultValue(true);

        builder.Property(x => x.IsDeleted)
               .HasDefaultValue(false);

        // Indexes
        builder.HasIndex(x => x.BuildingId);
        builder.HasIndex(x => x.ManagerUserId);

        builder.HasIndex(x => new { x.BuildingId, x.RoomName })
               .IsUnique();

        // Relationships
        builder.HasOne<Building>()
               .WithMany()
               .HasForeignKey(x => x.BuildingId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(x => x.ManagerUserId)
               .OnDelete(DeleteBehavior.Restrict);

        // Soft delete filter
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
