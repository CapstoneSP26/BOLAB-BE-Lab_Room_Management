using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations.Booking;

public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Feedback> builder)
    {
        // Table
        builder.ToTable("feedbacks");

        // Primary key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .ValueGeneratedNever();

        // Required relations
        builder.Property(x => x.BookingId)
               .IsRequired();

        builder.Property(x => x.UserId)
               .IsRequired();

        // Feedback content
        builder.Property(x => x.FeedbackType)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.Description)
               .IsRequired()
               .HasMaxLength(2000);

        // Status
        builder.Property(x => x.IsResolved)
               .IsRequired()
               .HasDefaultValue(false);

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
        builder.HasIndex(x => x.BookingId);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.IsResolved);
        builder.HasIndex(x => x.FeedbackType);

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
