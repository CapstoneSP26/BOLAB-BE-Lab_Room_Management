using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations;

public class LabRoomConfiguration : IEntityTypeConfiguration<LabRoom>
{
    public void Configure(EntityTypeBuilder<LabRoom> builder)
    {
        // 1. Cấu hình Tên bảng và Khóa chính
        builder.ToTable("LabRooms");
        builder.HasKey(lr => lr.Id);

        // 2. Cấu hình các thuộc tính cơ bản
        builder.Property(lr => lr.RoomName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(lr => lr.Location)
            .HasMaxLength(200);

        builder.Property(lr => lr.Description)
            .HasMaxLength(1000);

        // Mặc định các giá trị boolean
        builder.Property(lr => lr.IsActive)
            .HasDefaultValue(true);

        builder.Property(lr => lr.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(lr => lr.HasEquipment)
            .HasDefaultValue(false);

        builder.Property(lr => lr.Capacity)
            .HasDefaultValue(1)
            .IsRequired();

        // Số lượng ghi đè (OverrideNumber) - có thể dùng cho logic ưu tiên hoặc giới hạn đặc biệt
        builder.Property(lr => lr.OverrideNumber)
            .HasDefaultValue(0);

        // 3. Cấu hình Auditing (IAuditable, IUserTrackable)
        builder.Property(lr => lr.CreatedAt).IsRequired();
        builder.Property(lr => lr.CreatedBy).IsRequired();

        // 4. Global Query Filter (Soft Delete)
        // Rule: Tự động loại bỏ các phòng đã bị xóa khỏi mọi kết quả truy vấn
        builder.HasQueryFilter(lr => !lr.IsDeleted);

        // 5. Cấu hình Quan hệ (Relationships)

        // LabRoom - Building (N-1)
        builder.HasOne(lr => lr.Building)
            .WithMany() // Nếu Building có ICollection<LabRoom> Rooms thì thêm vào đây
            .HasForeignKey(lr => lr.BuildingId)
            .OnDelete(DeleteBehavior.Restrict);
        // Rule: Không cho phép xóa Tòa nhà nếu bên trong vẫn còn phòng Lab đang tồn tại.

        // LabRoom - LabOwner (1-N)
        builder.HasMany(lr => lr.LabOwners)
            .WithOne(lo => lo.LabRoom)
            .HasForeignKey(lo => lo.LabRoomId)
            .OnDelete(DeleteBehavior.Cascade);

        // LabRoom - RoomPolicy (1-N)
        builder.HasMany(lr => lr.RoomPolicies)
            .WithOne(lo => lo.LabRoom)
            .HasForeignKey(lo => lo.LabRoomId)
            .OnDelete(DeleteBehavior.Cascade);


        // 6. Cấu hình Index & Constraints (Tính duy nhất)

        // Rule: Trong một Tòa nhà, không được có 2 phòng trùng tên nhau
        builder.HasIndex(lr => new { lr.BuildingId, lr.RoomName, lr.IsDeleted })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false")
            .HasDatabaseName("UQ_LabRoom_Building_Name");

        // Index phục vụ tìm kiếm nhanh theo trạng thái và tòa nhà
        builder.HasIndex(lr => lr.BuildingId);
        builder.HasIndex(lr => lr.IsActive);
    }
}