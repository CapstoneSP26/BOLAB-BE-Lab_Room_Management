using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Infrastructure.Persistence.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            // Tên bảng
            builder.ToTable("Notifications");

            // Khóa chính
            builder.HasKey(n => n.Id);

            // Cấu hình các cột
            builder.Property(n => n.Id)
                .ValueGeneratedOnAdd();

            builder.Property(n => n.UserId)
                .IsRequired(false); // Cho phép NULL nếu là global

            builder.Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(n => n.Message)
                .IsRequired();

            builder.Property(n => n.Type)
                .HasMaxLength(50);

            builder.Property(n => n.IsRead)
                .HasDefaultValue(false);

            builder.Property(n => n.CreatedAt)
                .HasDefaultValueSql("NOW()");

            builder.Property(n => n.ReadAt)
                .IsRequired(false);

            // Metadata ánh xạ từ JSONB sang string
            builder.Property(n => n.Metadata)
                .HasColumnType("jsonb");

            builder.Property(n => n.IsGlobal)
                .HasDefaultValue(false);

            // Quan hệ với bảng Users
            builder.HasOne<User>()
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(n => n.UserId);
            builder.HasIndex(n => n.IsGlobal);
        }
    }
}
