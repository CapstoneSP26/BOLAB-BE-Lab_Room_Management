using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations.Booking;

public class BookingConfiguration : IEntityTypeConfiguration<Domain.Entities.Booking>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Booking> builder)
    {
        builder.ToTable("bookings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .ValueGeneratedNever();

        // Time range
        builder.Property(x => x.StartTime)
               .IsRequired();

        builder.Property(x => x.EndTime)
               .IsRequired();

        // Purpose
        builder.Property(x => x.Purpose)
               .IsRequired()
               .HasMaxLength(255);

        // Enums
        builder.Property(x => x.Status)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(30);

        builder.Property(x => x.ParticipantMode)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(30);

        // Flags
        builder.Property(x => x.IsCourseSchedule)
               .HasDefaultValue(false);

        builder.Property(x => x.IsDeleted)
               .HasDefaultValue(false);

        // Indexes (performance-critical)
        builder.HasIndex(x => new { x.RoomId, x.StartTime, x.EndTime });
        builder.HasIndex(x => x.BookedByUserId);
        builder.HasIndex(x => x.SemesterId);
        builder.HasIndex(x => x.SlotTypeId);

        // Relationships
        builder.HasOne<LabRoom>()
               .WithMany()
               .HasForeignKey(x => x.RoomId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Semester>()
               .WithMany()
               .HasForeignKey(x => x.SemesterId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(x => x.BookedByUserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<SlotType>()
               .WithMany()
               .HasForeignKey(x => x.SlotTypeId)
               .OnDelete(DeleteBehavior.Restrict);

        // Soft delete
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
