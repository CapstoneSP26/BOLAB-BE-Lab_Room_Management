using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations;

public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        // 1. Cấu hình Tên bảng và Khóa chính
        builder.ToTable("Schedules");
        builder.HasKey(s => s.Id);

        // 2. Cấu hình các thuộc tính cơ bản
        builder.Property(s => s.ScheduleType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.ScheduleStatus)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.StudentCount)
            .IsRequired(); 
        
        builder.Property(s => s.SubjectCode)
            .HasMaxLength(20);

        builder.Property(s => s.StartTime)
            .IsRequired();

        builder.Property(s => s.EndTime)
            .IsRequired();

        builder.Property(s => s.IsActive)
            .HasDefaultValue(true);

        builder.Property(s => s.IsDeleted)
            .HasDefaultValue(false);

        // 3. Cấu hình Auditing
        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.CreatedBy).IsRequired();

        // 4. Global Query Filter (Soft Delete)
        builder.HasQueryFilter(s => !s.IsDeleted);

        // 5. Cấu hình Quan hệ (Relationships)

        // Schedule - User (Lecturer) (N-1)
        builder.HasOne(s => s.User)
            .WithMany() // Một giảng viên có nhiều lịch dạy
            .HasForeignKey(s => s.LecturerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Schedule - LabRoom (N-1)
        builder.HasOne(s => s.LabRoom)
            .WithMany()
            .HasForeignKey(s => s.LabRoomId)
            .OnDelete(DeleteBehavior.Restrict);

        // Schedule - LabRoom (N-1)
        builder.HasOne(s => s.Booking)
            .WithOne()
            .HasForeignKey<Schedule>(s => s.BookingId)
            .OnDelete(DeleteBehavior.Restrict);

        // Schedule - Group (N-1)
        builder.HasOne(s => s.Group)
            .WithMany()
            .HasForeignKey(s => s.GroupId)
            .OnDelete(DeleteBehavior.Restrict);

        // Schedule - SlotType (N-1)
        builder.HasOne(s => s.SlotType)
            .WithMany()
            .HasForeignKey(s => s.SlotTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Schedule - Report (1-N)
        builder.HasMany(s => s.Reports)
            .WithOne(r => r.Schedule)
            .HasForeignKey(r => r.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        // 6. Cấu hình Hiệu năng và Ràng buộc (Performance & Index)

        // Tìm nhanh lịch trình của một Giảng viên cụ thể
        builder.HasIndex(s => s.LecturerId);

        // Index cho trạng thái để lọc lịch (VD: Active, Cancelled)
        builder.HasIndex(s => s.ScheduleStatus);
    }
}