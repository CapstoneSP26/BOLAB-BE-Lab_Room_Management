using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations;

public class EmailTemplateConfiguration : IEntityTypeConfiguration<EmailTemplate>
{
    public void Configure(EntityTypeBuilder<EmailTemplate> builder)
    {
        // 1. Tên bảng
        builder.ToTable("EmailTemplates");

        // 2. Khóa chính (Int - Tối ưu cho việc tìm kiếm template)
        builder.HasKey(et => et.Id);

        // 3. Cấu hình nội dung (Content)
        // Dùng kiểu 'text' trong PostgreSQL cho nội dung dài (HTML/Template)
        // Kiểu 'text' tối ưu hơn 'varchar' cho các chuỗi có độ dài không xác định
        builder.Property(et => et.Content)
            .IsRequired()
            .HasColumnType("text");

        // 4. Cấu hình Enum EmailType
        // Tối ưu Space: Lưu dưới dạng int (integer) thay vì string
        builder.Property(et => et.Type)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
    }
}