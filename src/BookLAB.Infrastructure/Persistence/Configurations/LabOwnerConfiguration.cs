using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations;

public class LabOwnerConfiguration : IEntityTypeConfiguration<LabOwner>
{
    public void Configure(EntityTypeBuilder<LabOwner> builder)
    {
        // 1. Cấu hình Tên bảng và Khóa chính
        builder.ToTable("LabOwners");
        builder.HasKey(lo => lo.Id);

        // 2. Cấu hình Quan hệ (Relationships)

        // LabOwner - User (N-1)
        builder.HasOne(lo => lo.User)
            .WithMany() // Một User có thể quản lý nhiều phòng Lab
            .HasForeignKey(lo => lo.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        // Rule: Nếu xóa User, các quyền sở hữu phòng Lab của User đó sẽ bị xóa theo.

        // LabOwner - LabRoom (N-1)
        builder.HasOne(lo => lo.LabRoom)
            .WithMany(lr => lr.LabOwners) // Giả định LabRoom có ICollection<LabOwner>
            .HasForeignKey(lo => lo.LabRoomId)
            .OnDelete(DeleteBehavior.Cascade);
        // Rule: Nếu xóa phòng Lab, các bản ghi chủ sở hữu liên quan sẽ bị xóa.

        // 3. Ràng buộc duy nhất (Unique Constraint)
        // Rule: Một User không thể được gán làm chủ sở hữu của cùng một phòng Lab 2 lần.
        builder.HasIndex(lo => new { lo.UserId, lo.LabRoomId })
            .IsUnique()
            .HasDatabaseName("UQ_LabOwner_User_Room");

        // 4. Index để tối ưu truy vấn

        // Tìm nhanh: "Phòng Lab này do những ai quản lý?"
        builder.HasIndex(lo => lo.LabRoomId)
            .HasDatabaseName("IX_LabOwner_LabRoomId");

        // Tìm nhanh: "User này đang quản lý những phòng Lab nào?"
        builder.HasIndex(lo => lo.UserId)
            .HasDatabaseName("IX_LabOwner_UserId");
    }
}