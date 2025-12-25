using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations.Booking;

public class BookingRequestConfiguration : IEntityTypeConfiguration<BookingRequest>
{
    public void Configure(EntityTypeBuilder<BookingRequest> builder)
    {
        // Table
        builder.ToTable("booking_requests");

        // Primary Key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .ValueGeneratedNever();

        // Required fields
        builder.Property(x => x.BookingId)
               .IsRequired();

        builder.Property(x => x.RequestedByUserId)
               .IsRequired();

        // Enum → string (safe for future)
        builder.Property(x => x.ApprovalStatus)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(30);

        // Optional approval info
        builder.Property(x => x.ApprovedByUserId)
               .IsRequired(false);

        builder.Property(x => x.ApprovalNotes)
               .HasMaxLength(500)
               .IsRequired(false);

        // Audit fields
        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.UpdatedAt)
               .IsRequired(false);

        builder.Property(x => x.CreatedBy)
               .IsRequired(false);

        builder.Property(x => x.UpdatedBy)
               .IsRequired(false);

        // Indexes (query performance)
        builder.HasIndex(x => x.BookingId);
        builder.HasIndex(x => x.ApprovalStatus);
        builder.HasIndex(x => x.RequestedByUserId);

        // Relationships
        builder.HasOne<Domain.Entities.Booking>()
               .WithMany()
               .HasForeignKey(x => x.BookingId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(x => x.RequestedByUserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(x => x.ApprovedByUserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
