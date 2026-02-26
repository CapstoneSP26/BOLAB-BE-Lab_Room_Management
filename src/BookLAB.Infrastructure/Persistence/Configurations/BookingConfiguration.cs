using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        // 1. Cấu hình Tên bảng và Khóa chính
        builder.ToTable("Bookings");
        builder.HasKey(b => b.Id);

        // 2. Cấu hình các thuộc tính cơ bản
        builder.Property(b => b.Reason)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(b => b.StartTime)
            .IsRequired();

        builder.Property(b => b.EndTime)
            .IsRequired();

        // Cấu hình Enums (Lưu dưới dạng String để dễ đọc trong DB hoặc Integer để tối ưu)
        builder.Property(b => b.BookingStatus)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(b => b.Recur)
            .HasDefaultValue(0);

        builder.Property(b => b.BookingType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(b => b.StudentCount)
            .IsRequired();

        // 3. Cấu hình các thuộc tính từ IAuditable & IUserTrackable
        builder.Property(b => b.CreatedAt).IsRequired();
        builder.Property(b => b.CreatedBy).IsRequired();
        // Note: UpdatedAt và UpdatedBy để Nullable theo Entity của bạn

        // 4. Cấu hình Quan hệ (Relationships)

        // Booking - LabRoom (1-n)
        builder.HasOne(b => b.LabRoom)
            .WithMany() // Nếu LabRoom không có ICollection<Booking> thì để trống
            .HasForeignKey(b => b.LabRoomId)
            .OnDelete(DeleteBehavior.Restrict); // Tránh xóa phòng làm mất lịch sử booking

        // Booking - PurposeType (1-n)
        builder.HasOne(b => b.PurposeType)
            .WithMany()
            .HasForeignKey(b => b.PurposeTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Booking - BookingGroups (1-n)
        builder.HasMany(b => b.BookingGroups)
            .WithOne(bg => bg.Booking)
            .HasForeignKey(bg => bg.BookingId)
            .OnDelete(DeleteBehavior.Cascade); // Xóa Booking thì xóa các nhóm liên quan

        // Booking - SlotType (1-n)
        builder.HasOne(b => b.SlotType)
            .WithMany()
            .HasForeignKey(b => b.SlotTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Booking - CreatedByUser (1-n)
        builder.HasOne(b => b.CreatedByUser)
            .WithMany()
            .HasForeignKey(b => b.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        // 5. Cấu hình Hiệu năng (Performance & Index)

        // Index cho LabRoomId và Khoảng thời gian để check trùng lịch cực nhanh
        builder.HasIndex(b => new { b.LabRoomId, b.StartTime, b.EndTime })
            .HasDatabaseName("IX_Booking_Room_Time");

        // Index cho Status để lọc các đơn "Pending" hoặc "Approved" nhanh hơn
        builder.HasIndex(b => b.BookingStatus);
    }
}