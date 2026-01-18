using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations;

public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
{
    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        // 1. Cấu hình Tên bảng và Khóa chính
        builder.ToTable("Attendances");
        builder.HasKey(a => a.Id);

        // 2. Cấu hình các thuộc tính thời gian
        // Cho phép Null vì lúc khởi tạo danh sách điểm danh chưa có thời gian thực tế
        builder.Property(a => a.CheckInTime)
            .IsRequired(false);

        builder.Property(a => a.CheckOutTime)
            .IsRequired(false);

        // 3. Cấu hình Enums
        builder.Property(a => a.CheckInMethod)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(a => a.AttendanceStatus)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // 4. Cấu hình các thuộc tính Auditing
        builder.Property(a => a.CreatedAt).IsRequired();
        builder.Property(a => a.CreatedBy).IsRequired();

        // 5. Cấu hình Quan hệ (Relationships)

        // Attendance - Booking (N-1)
        builder.HasOne(a => a.Booking)
            .WithMany() // Một buổi Booking có nhiều bản ghi điểm danh
            .HasForeignKey(a => a.BookingId)
            .OnDelete(DeleteBehavior.Cascade);
        // Nếu hủy buổi Booking thì xóa luôn dữ liệu điểm danh liên quan

        // Attendance - User (N-1)
        builder.HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // 6. Cấu hình Hiệu năng & Ràng buộc (Performance & Constraints)

        // Quan trọng: Một sinh viên chỉ có duy nhất 1 bản ghi điểm danh trong 1 buổi Booking
        builder.HasIndex(a => new { a.BookingId, a.UserId })
            .IsUnique()
            .HasDatabaseName("UQ_Attendance_Booking_User");

        // Index để giảng viên xuất báo cáo điểm danh theo buổi nhanh hơn
        builder.HasIndex(a => a.BookingId);

        // Index phục vụ việc thống kê tình trạng đi học của sinh viên
        builder.HasIndex(a => a.AttendanceStatus);
    }
}