using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Infrastructure.Persistence.Configurations
{
    public class ReportTypeConfiguration : IEntityTypeConfiguration<ReportType>
    {
        public void Configure(EntityTypeBuilder<ReportType> builder)
        {
            // Tên bảng
            builder.ToTable("ReportTypes");

            // Khóa chính
            builder.HasKey(rt => rt.ReportTypeId);

            // Thuộc tính ReportTypeId
            builder.Property(rt => rt.ReportTypeId)
                   .ValueGeneratedOnAdd();

            // Thuộc tính ReportTypeName
            builder.Property(rt => rt.ReportTypeName)
                   .IsRequired()
                   .HasMaxLength(200);

            // Quan hệ 1-n với Report
            builder.HasMany(rt => rt.Reports)
                   .WithOne(r => r.ReportType)
                   .HasForeignKey(r => r.ReportTypeId)
                   .OnDelete(DeleteBehavior.SetNull);
            // Nếu ReportType bị xóa, ReportTypeId trong Report sẽ null
        }
    }
}
