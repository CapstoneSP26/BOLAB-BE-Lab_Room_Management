using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations.Slot;

public class SlotTypeConfiguration : IEntityTypeConfiguration<SlotType>
{
    public void Configure(EntityTypeBuilder<SlotType> builder)
    {
        builder.ToTable("slot_types");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .ValueGeneratedNever();

        builder.Property(x => x.Code)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.HasIndex(x => x.Code)
               .IsUnique();

        builder.Property(x => x.DurationMinutes)
               .IsRequired(false);

        builder.Property(x => x.IsFixedDuration)
               .HasDefaultValue(true);

        builder.Property(x => x.RequiresApproval)
               .HasDefaultValue(false);

        builder.Property(x => x.AllowsOverCapacity)
               .HasDefaultValue(false);

        builder.Property(x => x.IsActive)
               .HasDefaultValue(true);
    }
}
