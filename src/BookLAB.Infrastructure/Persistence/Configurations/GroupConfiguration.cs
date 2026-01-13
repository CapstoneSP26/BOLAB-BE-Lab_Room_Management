using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        // 1. Cấu hình Tên bảng và Khóa chính
        builder.ToTable("Groups");
        builder.HasKey(g => g.Id);

        // 2. Cấu hình thuộc tính
        builder.Property(g => g.GroupName)
            .HasMaxLength(150)
            .IsRequired();

        // 3. Cấu hình các thuộc tính Auditing & Soft Delete
        builder.Property(g => g.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(g => g.CreatedAt).IsRequired();
        builder.Property(g => g.CreatedBy).IsRequired();

        // 4. Global Query Filter cho Soft Delete
        // Rule: Mặc định mọi câu truy vấn sẽ tự động bỏ qua các nhóm đã bị xóa tạm
        builder.HasQueryFilter(g => !g.IsDeleted);

        // 5. Cấu hình Quan hệ (Relationships)

        // Group - User (Owner) (N-1)
        builder.HasOne(g => g.User)
            .WithMany() // Một User (Giảng viên) có thể sở hữu nhiều Group
            .HasForeignKey(g => g.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
        // Rule: Không cho phép xóa User nếu User đó đang là chủ sở hữu của một Group

        // 6. Cấu hình Index & Constraints

        // Tối ưu tìm kiếm theo tên nhóm
        builder.HasIndex(g => g.GroupName);

        // Tìm nhanh các nhóm thuộc sở hữu của một Giảng viên
        builder.HasIndex(g => g.OwnerId);

        // Ràng buộc: Một giảng viên không nên tạo 2 nhóm trùng tên nhau
        builder.HasIndex(g => new { g.OwnerId, g.GroupName, g.IsDeleted })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false")
            .HasDatabaseName("UQ_Group_Owner_Name_Active");
    }
}