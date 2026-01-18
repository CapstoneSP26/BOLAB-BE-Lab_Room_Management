using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations;

public class PurposeTypeConfiguration : IEntityTypeConfiguration<PurposeType>
{
    public void Configure(EntityTypeBuilder<PurposeType> builder)
    {
        // 1. Cấu hình Tên bảng và Khóa chính
        builder.ToTable("PurposeTypes");
        builder.HasKey(p => p.Id);

        // 2. Cấu hình thuộc tính
        builder.Property(p => p.PurposeName)
            .HasMaxLength(100)
            .IsRequired();

        // 3. Ràng buộc duy nhất (Unique Constraint)
        // Rule: Không cho phép tạo 2 loại mục đích trùng tên nhau (ví dụ: đã có 'Workshop' thì không tạo thêm 'Workshop' nữa)
        builder.HasIndex(p => p.PurposeName)
            .IsUnique()
            .HasDatabaseName("UQ_PurposeType_Name");

    }
}