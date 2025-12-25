using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations.Attendance;

public class AttendanceSummaryConfiguration : IEntityTypeConfiguration<AttendanceSummary>
{
    public void Configure(EntityTypeBuilder<AttendanceSummary> builder)
    {
        // Table
        builder.ToTable("attendance_summaries");

        // Primary Key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .ValueGeneratedNever();

        // Required fields
        builder.Property(x => x.BookingId)
               .IsRequired();

        builder.Property(x => x.RecordedByUserId)
               .IsRequired();

        builder.Property(x => x.TotalParticipants)
               .IsRequired();

        // Enum → string
        builder.Property(x => x.AttendanceMode)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(30);

        // Optional fields
        builder.Property(x => x.TotalGroups)
               .IsRequired(false);

        // Audit fields
        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.UpdatedAt)
               .IsRequired(false);

        // Indexes
        builder.HasIndex(x => x.BookingId)
               .IsUnique(); // 1 summary per booking

        builder.HasIndex(x => x.AttendanceMode);

        // Relationships
        builder.HasOne<Domain.Entities.Booking>()
               .WithMany()
               .HasForeignKey(x => x.BookingId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(x => x.RecordedByUserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
