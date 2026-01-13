using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations;

public class BookingGroupConfiguration : IEntityTypeConfiguration<BookingGroup>
{
    public void Configure(EntityTypeBuilder<BookingGroup> builder)
    {
        // 1. Cấu hình Tên bảng và Khóa chính
        builder.ToTable("BookingGroups");
        builder.HasKey(bg => bg.Id);

        // 2. Cấu hình Quan hệ (Relationships)

        // Quan hệ với Booking (N-1)
        builder.HasOne(bg => bg.Booking)
            .WithMany(b => b.BookingGroups)
            .HasForeignKey(bg => bg.BookingId)
            .OnDelete(DeleteBehavior.Cascade);
        // Rule: Nếu xóa đơn Booking, các liên kết nhóm này phải bị xóa theo (Cascade)

        // Quan hệ với Group (N-1)
        builder.HasOne(bg => bg.Group)
            .WithMany() // Giả định Group không cần list BookingGroups ngược lại
            .HasForeignKey(bg => bg.GroupId)
            .OnDelete(DeleteBehavior.Restrict);
        // Rule: Không cho phép xóa một Nhóm (Group) nếu nhóm đó đang có lịch Booking hiện tại

        // 3. Ràng buộc duy nhất (Unique Constraint)
        // Rule: Trong một đơn Booking, không được phép add trùng một Group 2 lần.
        builder.HasIndex(bg => new { bg.BookingId, bg.GroupId })
            .IsUnique()
            .HasDatabaseName("UQ_Booking_Group");

        // 4. Index để tối ưu truy vấn
        // Giúp tìm nhanh "Nhóm này có những buổi Booking nào?"
        builder.HasIndex(bg => bg.GroupId)
            .HasDatabaseName("IX_BookingGroup_GroupId");
    }
}