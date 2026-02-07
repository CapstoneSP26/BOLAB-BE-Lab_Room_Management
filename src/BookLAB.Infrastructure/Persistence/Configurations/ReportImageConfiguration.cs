using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations;

public class ReportImageConfiguration : IEntityTypeConfiguration<ReportImage>
{
    public void Configure(EntityTypeBuilder<ReportImage> builder)
    {
        // 1. Tên bảng
        builder.ToTable("ReportImages");

        // 2. Khóa chính (Int từ BaseEntity) - Tối ưu cho Clustered Index
        builder.HasKey(ri => ri.Id);

        // 3. Cấu hình các thuộc tính
        builder.Property(ri => ri.ImageUrl)
            .IsRequired()
            .HasMaxLength(1000); // Giới hạn độ dài để tránh tốn Space trong Index

        builder.Property(ri => ri.Size)
            .IsRequired();

        // Chuyển đổi Enum sang Int để tiết kiệm Space Complexity (4 bytes thay vì chuỗi string)
        builder.Property(ri => ri.FileType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(ri => ri.IsAvatar)
            .HasDefaultValue(false);

        // 4. Cấu hình Quan hệ (N-1) với Report (Khóa ngoại Guid)
        builder.HasOne(ri => ri.Report)
            .WithMany(r => r.ReportImages)
            .HasForeignKey(ri => ri.ReportId)
            .OnDelete(DeleteBehavior.Cascade);

        // 5. Tối ưu Time Complexity bằng Indexing
        // Vì ReportId là Guid (không tuần tự), Index này cực kỳ quan trọng 
        // để tránh Table Scan khi lấy danh sách ảnh của một báo cáo.
        builder.HasIndex(ri => ri.ReportId)
            .HasDatabaseName("IX_ReportImage_ReportId");

        // Index bổ sung nếu bạn thường xuyên lọc ảnh chính của báo cáo
        builder.HasIndex(ri => new { ri.ReportId, ri.IsAvatar })
            .HasDatabaseName("IX_ReportImage_Report_Avatar");
    }
}