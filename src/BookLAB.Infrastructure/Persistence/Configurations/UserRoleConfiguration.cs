using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        // 1. Cấu hình Tên bảng và Khóa chính
        builder.ToTable("UserRoles");
        builder.HasKey(ur => new { ur.UserId, ur.RoleId });

        // 2. Cấu hình Quan hệ (Relationships)

        // UserRole - User (N-1)
        builder.HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        // Rule: Nếu xóa tài khoản User, các liên kết phân quyền của họ phải bị xóa theo.

        // UserRole - Role (N-1)
        builder.HasOne(ur => ur.Role)
            .WithMany() // Một Role có thể được gán cho rất nhiều User
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
        // Rule: Nếu xóa một Role trong hệ thống, các liên kết gán quyền cũng biến mất.

        // Tìm nhanh: "User này có những quyền gì?" (Dùng khi tạo Token/Claims)
        builder.HasIndex(ur => ur.UserId)
            .HasDatabaseName("IX_UserRole_UserId");

        // Tìm nhanh: "Những ai đang có quyền Admin?"
        builder.HasIndex(ur => ur.RoleId)
            .HasDatabaseName("IX_UserRole_RoleId");
    }
}