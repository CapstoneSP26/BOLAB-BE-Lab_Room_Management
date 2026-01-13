using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations;

public class RoomPolicyConfiguration : IEntityTypeConfiguration<RoomPolicy>
{
    public void Configure(EntityTypeBuilder<RoomPolicy> builder)
    {
        // 1. Cấu hình Tên bảng và Khóa chính
        builder.ToTable("RoomPolicies");
        builder.HasKey(rp => rp.Id);

        // 2. Cấu hình các thuộc tính cơ bản
        builder.Property(rp => rp.PolicyKey)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(rp => rp.PolicyValue)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(rp => rp.IsActive)
            .HasDefaultValue(true);

        // 3. Cấu hình Auditing (IAuditable, IUserTrackable)
        builder.Property(rp => rp.CreatedAt).IsRequired();
        builder.Property(rp => rp.CreatedBy).IsRequired();


        // 4. Cấu hình Ràng buộc & Index (Quan trọng)

        // Rule: Trong một phòng Lab, không được phép có 2 Key trùng nhau (VD: Không thể có 2 cái 'MaxCapacity')
        builder.HasIndex(rp => new { rp.LabRoomId, rp.PolicyKey })
            .IsUnique()
            .HasDatabaseName("UQ_Room_PolicyKey");

        // Index cho PolicyKey để tìm kiếm nhanh các phòng có cùng một loại chính sách
        builder.HasIndex(rp => rp.PolicyKey);

        // Index cho IsActive để chỉ lấy các chính sách đang có hiệu lực
        builder.HasIndex(rp => rp.IsActive);
    }
}