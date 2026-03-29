using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations;

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        // 1. Cấu hình Tên bảng và Khóa chính
        builder.ToTable("Reports");
        builder.HasKey(r => r.Id);

        // 2. Cấu hình các thuộc tính cơ bản
        builder.Property(r => r.Description)
            .HasMaxLength(2000) // Báo cáo sự cố cần không gian mô tả chi tiết
            .IsRequired();

        builder.Property(r => r.IsResolved)
            .HasDefaultValue(false);


        // 3. Cấu hình Auditing (IAuditable, IUserTrackable)
        builder.Property(r => r.CreatedAt).IsRequired();
        builder.Property(r => r.CreatedBy).IsRequired();

        // 4. Cấu hình Quan hệ (Relationships)

        // Report - Schedule (N-1)
        // Một buổi học (Schedule) có thể có nhiều báo cáo (VD: hỏng chuột và hỏng đèn)
        builder.HasOne(r => r.Schedule)
            .WithMany(s => s.Reports) // Nếu Schedule có ICollection<Report> thì thêm vào đây
            .HasForeignKey(r => r.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);
        // Rule: Nếu xóa lịch học (Schedule) thì các báo cáo đi kèm cũng bị xóa. 
        // Tuy nhiên, nếu dùng SoftDelete cho Schedule thì báo cáo vẫn tồn tại.


        // Report - ReportType (N-1)
        builder.HasOne(r => r.ReportType)
            .WithMany(s => s.Reports)
            .HasForeignKey(r => r.ReportTypeId)
            .OnDelete(DeleteBehavior.SetNull);

        // 5. Cấu hình Index để tối ưu quản lý

        // Tìm nhanh các báo cáo chưa được xử lý (Dành cho Dashboard của Lab Manager)
        builder.HasIndex(r => r.IsResolved)
            .HasFilter("\"IsResolved\" = false")
            .HasDatabaseName("IX_Report_Unresolved");

        // Tìm báo cáo theo loại (VD: lọc tất cả báo cáo về 'TechnicalIssue')
        builder.HasIndex(r => r.ReportTypeId);

        // Tìm báo cáo theo người tạo
        builder.HasIndex(r => r.CreatedBy);
    }
}