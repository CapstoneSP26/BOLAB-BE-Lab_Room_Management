using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations;

public class SlotFrameConfiguration : IEntityTypeConfiguration<SlotFrame>
{
    public void Configure(EntityTypeBuilder<SlotFrame> builder)
    {
        // 1. Cấu hình Tên bảng và Khóa chính
        builder.ToTable("SlotFrames");
        builder.HasKey(sf => sf.Id);

        // 2. Cấu hình các thuộc tính thời gian
        // Lưu ý: TimeOnly trong EF Core thường được ánh xạ thành "time" hoặc "time without time zone" trong PostgreSQL
        builder.Property(sf => sf.StartTimeSlot)
            .IsRequired();

        builder.Property(sf => sf.EndTimeSlot)
            .IsRequired();

        // Thứ tự của Slot (ví dụ: Slot 1, Slot 2...)
        builder.Property(sf => sf.OrderIndex)
            .IsRequired()
            .HasDefaultValue(0);

        // 3. Cấu hình Quan hệ (Relationships)

        // SlotFrame - SlotType (N-1)
        builder.HasOne(sf => sf.SlotType)
            .WithMany(st => st.SlotFrames)
            .HasForeignKey(sf => sf.SlotTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        // 4. Ràng buộc và Index (Constraints)

        // Rule: Trong cùng một SlotType, không được có 2 khung giờ trùng lặp thứ tự OrderIndex
        builder.HasIndex(sf => new { sf.SlotTypeId, sf.OrderIndex })
            .IsUnique()
            .HasDatabaseName("UQ_SlotFrame_Type_Order");

        // Rule: Trong cùng một SlotType, không nên có 2 khung giờ bắt đầu trùng nhau
        builder.HasIndex(sf => new { sf.SlotTypeId, sf.StartTimeSlot })
            .IsUnique()
            .HasDatabaseName("UQ_SlotFrame_Type_StartTime");

        // Index để truy vấn nhanh danh sách khung giờ của một loại Slot
        builder.HasIndex(sf => sf.SlotTypeId);
    }
}