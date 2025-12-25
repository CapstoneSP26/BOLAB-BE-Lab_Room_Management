using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations.Booking;

public class BookingUserConfiguration : IEntityTypeConfiguration<BookingUser>
{
    public void Configure(EntityTypeBuilder<BookingUser> builder)
    {
        builder.ToTable("booking_users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .ValueGeneratedNever();

        // Indexes
        builder.HasIndex(x => x.BookingId);
        builder.HasIndex(x => x.UserId);

        // Avoid duplicate user in one booking
        builder.HasIndex(x => new { x.BookingId, x.UserId })
               .IsUnique();

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
