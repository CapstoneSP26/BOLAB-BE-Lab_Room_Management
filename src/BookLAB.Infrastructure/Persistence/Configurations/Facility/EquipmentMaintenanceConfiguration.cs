using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations.Facility;

public class EquipmentMaintenanceConfiguration
    : IEntityTypeConfiguration<EquipmentMaintenance>
{
    public void Configure(EntityTypeBuilder<EquipmentMaintenance> builder)
    {
        // Table
        builder.ToTable("equipment_maintenances");

        // Primary key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .ValueGeneratedNever();

        // Required fields
        builder.Property(x => x.EquipmentItemId)
               .IsRequired();

        builder.Property(x => x.ReportedByUserId)
               .IsRequired();

        builder.Property(x => x.Description)
               .IsRequired()
               .HasMaxLength(2000);

        // Enum → string
        builder.Property(x => x.Status)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(30);

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
        builder.HasIndex(x => x.EquipmentItemId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.ReportedByUserId);

        // Relationships
        builder.HasOne<EquipmentItem>()
               .WithMany()
               .HasForeignKey(x => x.EquipmentItemId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(x => x.ReportedByUserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
