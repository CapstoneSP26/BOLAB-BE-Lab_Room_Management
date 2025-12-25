using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations.Facility;

public class EquipmentItemConfiguration : IEntityTypeConfiguration<EquipmentItem>
{
    public void Configure(EntityTypeBuilder<EquipmentItem> builder)
    {
        // Table
        builder.ToTable("equipment_items");

        // Primary key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .ValueGeneratedNever();

        // Item name
        builder.Property(x => x.ItemName)
               .IsRequired()
               .HasMaxLength(200);

        // Serial number
        builder.Property(x => x.SerialNumber)
               .IsRequired()
               .HasMaxLength(100);

        builder.HasIndex(x => x.SerialNumber)
               .IsUnique();

        // Room (optional)
        builder.Property(x => x.RoomId)
               .IsRequired(false);

        // Enum → string
        builder.Property(x => x.Condition)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(30);

        // Soft delete
        builder.Property(x => x.IsDeleted)
               .IsRequired()
               .HasDefaultValue(false);

        // Indexes
        builder.HasIndex(x => x.RoomId);
        builder.HasIndex(x => x.Condition);
        builder.HasIndex(x => x.IsDeleted);

        // Relationships
        builder.HasOne<LabRoom>()
               .WithMany()
               .HasForeignKey(x => x.RoomId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
