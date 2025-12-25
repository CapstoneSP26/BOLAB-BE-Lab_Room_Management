using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations.Attendance;

public class AttendanceDetailConfiguration : IEntityTypeConfiguration<AttendanceDetail>
{
    public void Configure(EntityTypeBuilder<AttendanceDetail> builder)
    {
        // Table
        builder.ToTable("attendance_details");

        // Primary key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .ValueGeneratedNever();

        // Required fields
        builder.Property(x => x.BookingId)
               .IsRequired();

        builder.Property(x => x.UserId)
               .IsRequired();

        builder.Property(x => x.CheckInTime)
               .IsRequired();

        // Enum → string
        builder.Property(x => x.CheckInMethod)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(30);

        // Optional fields
        builder.Property(x => x.CheckOutTime)
               .IsRequired(false);

        // Audit fields
        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.UpdatedAt)
               .IsRequired(false);

        // Indexes
        builder.HasIndex(x => x.BookingId);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => new { x.BookingId, x.UserId });

        // Relationships
        builder.HasOne<Domain.Entities.Booking>()
               .WithMany()
               .HasForeignKey(x => x.BookingId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
