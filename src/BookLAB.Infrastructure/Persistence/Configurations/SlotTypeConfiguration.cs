using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations;

public class SlotTypeConfiguration : IEntityTypeConfiguration<SlotType>
{
    public void Configure(EntityTypeBuilder<SlotType> builder)
    {
        // 1. Cấu hình Tên bảng và Khóa chính
        builder.ToTable("SlotTypes");
        builder.HasKey(st => st.Id);

        // 2. Cấu hình các thuộc tính
        builder.Property(st => st.Code)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(st => st.Name)
            .HasMaxLength(100)
            .IsRequired();

        // 3. Cấu hình Quan hệ (Relationships)

        // SlotType - Campus (N-1)
        builder.HasOne(st => st.Campus)
            .WithMany() // Nếu Campus có ICollection<SlotType> thì thêm vào đây
            .HasForeignKey(st => st.CampusId)
            .OnDelete(DeleteBehavior.Cascade);
        // Rule: Nếu xóa Campus, các định nghĩa Slot của Campus đó cũng bị xóa.

        // SlotType - SlotFrame (1-N)
        builder.HasMany(st => st.SlotFrames)
            .WithOne(sf => sf.SlotType)
            .HasForeignKey(sf => sf.SlotTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        // 4. Ràng buộc và Index (Constraints)

        // Rule: Trong cùng một Campus, mã Code (VD: 'S90', 'S120') phải là duy nhất
        builder.HasIndex(st => new { st.CampusId, st.Code })
            .IsUnique()
            .HasDatabaseName("UQ_SlotType_Campus_Code");

        // Index để tìm kiếm nhanh SlotType theo Campus
        builder.HasIndex(st => st.CampusId);
    }
}