using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations
{
    public class ImportBatchConfiguration : IEntityTypeConfiguration<ImportBatch>
    {
        public void Configure(EntityTypeBuilder<ImportBatch> builder)
        {
            // Table
            builder.ToTable("ImportBatches");

            // Primary key
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            // Name
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(255);

            // ImportBatchType (enum → string)
            builder.Property(x => x.ImportBatchType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            // SemesterName
            builder.Property(x => x.SemesterName)
                .IsRequired()
                .HasMaxLength(100);

            // Soft delete
            builder.Property(x => x.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            // Audit fields
            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt);

            // User tracking
            builder.Property(x => x.CreatedBy);

            builder.Property(x => x.UpdatedBy);

            // Indexes (rất quan trọng)
            builder.HasIndex(x => x.CreatedAt);

            builder.HasIndex(x => x.SemesterName);

            builder.HasIndex(x => x.ImportBatchType);

            // Optional: query filter cho soft delete
            builder.HasQueryFilter(x => !x.IsDeleted);

            builder.HasIndex(x => new { x.Name, x.SemesterName })
                .IsUnique();
        }
    }
}
