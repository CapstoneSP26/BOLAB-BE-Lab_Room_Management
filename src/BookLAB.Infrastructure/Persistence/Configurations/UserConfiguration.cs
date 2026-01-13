using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // 1. Cấu hình Tên bảng và Khóa chính
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);

        // 2. Cấu hình các thuộc tính định danh
        builder.Property(u => u.Email)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(u => u.FullName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.UserImageUrl)
            .HasMaxLength(2048)
            .IsRequired(false);

        // 3. Cấu hình Trạng thái và Soft Delete
        builder.Property(u => u.IsActive)
            .HasDefaultValue(true);

        builder.Property(u => u.IsDeleted)
            .HasDefaultValue(false);

        // 4. Cấu hình Auditing (IAuditable, IUserTrackable)
        builder.Property(u => u.CreatedAt).IsRequired();
        builder.Property(u => u.CreatedBy).IsRequired(false); // CreatedBy null khi User tự đăng ký

        // 5. Global Query Filter (Soft Delete)
        // Rule: Tự động loại bỏ người dùng đã bị xóa khỏi các danh sách hiển thị
        builder.HasQueryFilter(u => !u.IsDeleted);

        // 6. Cấu hình Quan hệ (Relationships)

        // User - Campus (N-1)
        builder.HasOne(u => u.Campus)
            .WithMany() // Một Campus có nhiều User
            .HasForeignKey(u => u.CampusId)
            .OnDelete(DeleteBehavior.Restrict);
        // Rule: Không cho xóa Campus nếu vẫn còn User thuộc Campus đó

        // User - UserRole (1-N) - Cấu hình Identity/RBAC
        builder.HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // 7. Ràng buộc dữ liệu & Index (Performance)

        // Rule quan trọng: Email phải là duy nhất trên toàn hệ thống
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("UQ_User_Email");

        // Index để tìm kiếm nhanh User theo Campus
        builder.HasIndex(u => u.CampusId);

        // Index cho tên để hỗ trợ tìm kiếm/gợi ý giảng viên
        builder.HasIndex(u => u.FullName);
    }
}