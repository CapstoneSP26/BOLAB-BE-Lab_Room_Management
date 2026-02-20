using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations;

public class LabImageConfiguration : IEntityTypeConfiguration<LabImage>
{
    public void Configure(EntityTypeBuilder<LabImage> builder)
    {
        // 1. Tên bảng
        builder.ToTable("LabImages");

        // 2. Khóa chính (Int - Tối ưu Space/Time)
        builder.HasKey(li => li.Id);

        // 3. Cấu hình thuộc tính
        builder.Property(li => li.ImageUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(li => li.Size)
            .IsRequired();

        // Lưu Enum dưới dạng String để dễ đọc trong DB hoặc Int để tối ưu Space
        // Ưu tiên Space Complexity: Lưu dạng SmallInt/Int
        builder.Property(li => li.FileType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(li => li.IsAvatar)
            .HasDefaultValue(false);

        // 4. Cấu hình Quan hệ (N-1 với LabRoom)
        builder.HasOne(li => li.LabRoom)
            .WithMany(lr => lr.LabImages)
            .HasForeignKey(li => li.LabRoomId)
            .OnDelete(DeleteBehavior.Cascade);

        // 5. Index để tối ưu Time Complexity
        // Index này giúp truy vấn "Lấy tất cả ảnh của một phòng Lab" cực nhanh
        builder.HasIndex(li => li.LabRoomId)
            .HasDatabaseName("IX_LabImage_LabRoomId");

        // Index bổ sung: Nếu thường xuyên lọc ảnh Avatar của phòng
        builder.HasIndex(li => new { li.LabRoomId, li.IsAvatar })
            .HasDatabaseName("IX_LabImage_Room_Avatar");
    }
}