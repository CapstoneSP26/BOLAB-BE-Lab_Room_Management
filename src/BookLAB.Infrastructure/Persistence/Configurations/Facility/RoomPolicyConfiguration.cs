using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations.Facility;

public class RoomPolicyConfiguration : IEntityTypeConfiguration<RoomPolicy>
{
    public void Configure(EntityTypeBuilder<RoomPolicy> builder)
    {
        // Table
        builder.ToTable("room_policies");

        // Primary key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .ValueGeneratedNever();

        // Required fields
        builder.Property(x => x.LabRoomId)
               .IsRequired();

        builder.Property(x => x.PolicyKey)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.PolicyValue)
               .IsRequired()
               .HasMaxLength(500);

        // Indexes
        builder.HasIndex(x => x.LabRoomId);
        builder.HasIndex(x => new { x.LabRoomId, x.PolicyKey })
               .IsUnique();

        // Relationships
        builder.HasOne<LabRoom>()
               .WithMany()
               .HasForeignKey(x => x.LabRoomId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
