using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations;

public class UserNotificationPreferenceConfiguration : IEntityTypeConfiguration<UserNotificationPreference>
{
    public void Configure(EntityTypeBuilder<UserNotificationPreference> builder)
    {
        builder.ToTable("UserNotificationPreferences");

        builder.HasKey(x => x.UserId);

        builder.Property(x => x.EmailNotifications)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(x => x.PushNotifications)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(x => x.BookingApproved)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(x => x.BookingRejected)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(x => x.BookingReminder)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithOne(u => u.NotificationPreference)
            .HasForeignKey<UserNotificationPreference>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
