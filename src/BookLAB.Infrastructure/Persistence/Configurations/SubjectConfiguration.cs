using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations
{
    public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
    {
        public void Configure(EntityTypeBuilder<Subject> builder)
        {
            // 1. Khóa chính
            builder.HasKey(x => x.Id);

            // 2. Cấu hình thuộc tính
            builder.Property(x => x.SubjectCode)
                .IsRequired()
                .HasMaxLength(20) // Mã môn học thường ngắn (PRN211, SWP391)
                .IsUnicode(false); // Thường là ký tự không dấu, dùng varchar để tối ưu

            builder.Property(x => x.SubjectName)
                .IsRequired()
                .HasMaxLength(200);

            // 3. Đảm bảo mã môn học là duy nhất (Unique Index)
            // Đây là chìa khóa để chống trùng lặp khi Import
            builder.HasIndex(x => x.SubjectCode)
                .IsUnique();

            // 5. Cấu hình bảng
            builder.ToTable("Subjects");
        }
    }
}