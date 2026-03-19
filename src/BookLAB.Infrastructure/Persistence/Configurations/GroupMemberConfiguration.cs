using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations;

public class GroupMemberConfiguration : IEntityTypeConfiguration<GroupMember>
{
    public void Configure(EntityTypeBuilder<GroupMember> builder)
    {
        // 1. Cấu hình Tên bảng và Khóa chính
        builder.ToTable("GroupMembers");
        builder.HasKey(gm => gm.Id);

        builder.Property(gm => gm.SubjectCode)
            .IsRequired(false)
            .HasMaxLength(20);

        // 2. Cấu hình Quan hệ (Relationships)

        // GroupMember - Group (N-1)
        builder.HasOne(gm => gm.Group)
            .WithMany() // Nếu trong Group có ICollection<GroupMember> Members thì đổi thành .WithMany(g => g.Members)
            .HasForeignKey(gm => gm.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
        // Rule: Nếu xóa Nhóm, danh sách thành viên trong nhóm đó tự động bị xóa theo.

        // GroupMember - User (N-1)
        builder.HasOne(gm => gm.User)
            .WithMany() // Một sinh viên có thể thuộc nhiều nhóm
            .HasForeignKey(gm => gm.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        // Rule: Nếu xóa tài khoản User, các liên kết thành viên nhóm của họ cũng bị xóa.

        // 3. Ràng buộc duy nhất (Unique Constraint)
        // Rule: Một sinh viên không thể tham gia vào cùng một nhóm 2 lần.
        builder.HasIndex(gm => new { gm.GroupId, gm.UserId })
            .IsUnique()
            .HasDatabaseName("UQ_Group_User_Member");

        // 4. Index để tối ưu truy vấn

        // Giúp truy vấn nhanh: "Sinh viên này đang tham gia những nhóm nào?"
        builder.HasIndex(gm => gm.UserId)
            .HasDatabaseName("IX_GroupMember_UserId");

        // Giúp truy vấn nhanh: "Danh sách sinh viên trong nhóm X"
        builder.HasIndex(gm => gm.GroupId)
            .HasDatabaseName("IX_GroupMember_GroupId");
    }
}