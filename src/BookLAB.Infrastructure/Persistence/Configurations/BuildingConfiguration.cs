using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations;

public class BuildingConfiguration : IEntityTypeConfiguration<Building>
{
    public void Configure(EntityTypeBuilder<Building> builder)
    {
        // 1. Cấu hình Tên bảng và Khóa chính
        builder.ToTable("Buildings");
        builder.HasKey(b => b.Id);

        // 2. Cấu hình các thuộc tính
        builder.Property(b => b.BuildingName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(b => b.BuildingCode)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(b => b.Description)
            .HasMaxLength(500)
            .IsRequired(false); // Cho phép null nếu không có mô tả

        builder.Property(b => b.BuildingImageUrl)
            .HasMaxLength(2048) // URL dài để chứa link Cloud/S3
            .IsRequired(false);

        // 3. Cấu hình Quan hệ (Relationships)

        // Building - Campus (N-1)
        builder.HasOne(b => b.Campus)
            .WithMany(c => c.Buildings) // Một cơ sở có nhiều tòa nhà
            .HasForeignKey(b => b.CampusId)
            .OnDelete(DeleteBehavior.Cascade);
        // Rule: Nếu xóa Campus (ví dụ Campus bị dỡ bỏ), các Tòa nhà thuộc Campus đó sẽ bị xóa theo.

        // 4. Ràng buộc dữ liệu & Index

        // Đảm bảo trong một Campus không có 2 tòa nhà trùng tên nhau
        builder.HasIndex(b => new { b.CampusId, b.BuildingName })
            .IsUnique()
            .HasDatabaseName("UQ_Building_Campus_Name");

        // Index cho CampusId để lọc danh sách tòa nhà theo Campus nhanh hơn
        builder.HasIndex(b => b.CampusId);
    }
}