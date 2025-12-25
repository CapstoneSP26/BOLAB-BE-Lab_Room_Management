using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations.Facility;

public class RequiredEquipmentConfiguration : IEntityTypeConfiguration<RequiredEquipment>
{
    public void Configure(EntityTypeBuilder<RequiredEquipment> builder)
    {
        // Table
        builder.ToTable("required_equipments");

        // Primary key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .ValueGeneratedNever();

        // Booking
        builder.Property(x => x.BookingId)
               .IsRequired();

        // Equipment name
        builder.Property(x => x.EquipmentName)
               .IsRequired()
               .HasMaxLength(200);

        // Quantity
        builder.Property(x => x.QuantityRequired)
               .IsRequired();

        // Optional equipment item (assigned after approval)
        builder.Property(x => x.EquipmentItemId)
               .IsRequired(false);

        // Indexes
        builder.HasIndex(x => x.BookingId);
        builder.HasIndex(x => x.EquipmentItemId);

        // Relationships
        builder.HasOne<Domain.Entities.Booking>()
               .WithMany()
               .HasForeignKey(x => x.BookingId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<EquipmentItem>()
               .WithMany()
               .HasForeignKey(x => x.EquipmentItemId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
