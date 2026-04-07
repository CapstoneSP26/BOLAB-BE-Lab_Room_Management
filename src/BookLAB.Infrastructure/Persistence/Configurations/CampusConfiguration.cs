using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations;

public class CampusConfiguration : IEntityTypeConfiguration<Campus>
{
    public void Configure(EntityTypeBuilder<Campus> builder)
    {
        // 1. Cấu hình Tên bảng và Khóa chính
        builder.ToTable("Campuses");
        builder.HasKey(c => c.Id);

        // 2. Cấu hình các thuộc tính
        builder.Property(c => c.CampusName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(c => c.CampusCode)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(c => c.Address)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(c => c.CampusImageUrl)
            .HasMaxLength(2048)
            .IsRequired(false);

        // Mặc định là True (đang hoạt động)
        builder.Property(c => c.IsActive)
            .HasDefaultValue(true);

        // 3. Cấu hình Quan hệ (Relationships)

        // Campus - Building (1-N)
        // Lưu ý: Quan hệ này đã được định nghĩa ở BuildingConfiguration, 
        // nhưng khai báo lại ở đây để đảm bảo tính tường minh.
        builder.HasMany(c => c.Buildings)
            .WithOne(b => b.Campus)
            .HasForeignKey(b => b.CampusId)
            .OnDelete(DeleteBehavior.Cascade);

        // 4. Ràng buộc dữ liệu & Index

        // Tên Campus phải là duy nhất trên toàn hệ thống
        builder.HasIndex(c => c.CampusName)
            .IsUnique()
            .HasDatabaseName("UQ_Campus_Name");

        // Index cho IsActive để lọc nhanh các Campus đang mở cửa
        builder.HasIndex(c => c.IsActive);
    }
}