using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations.Academic;

public class SemesterConfiguration : IEntityTypeConfiguration<Semester>
{
    public void Configure(EntityTypeBuilder<Semester> builder)
    {
        // Table name
        builder.ToTable("semesters");

        // Primary key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .ValueGeneratedNever();

        // Semester code (e.g. 2024-1, 2024-2)
        builder.Property(x => x.SemesterCode)
               .IsRequired()
               .HasMaxLength(20);

        builder.HasIndex(x => x.SemesterCode)
               .IsUnique();

        // Dates
        builder.Property(x => x.StartDate)
               .IsRequired();

        builder.Property(x => x.EndDate)
               .IsRequired();

        // Current semester flag
        builder.Property(x => x.IsCurrent)
               .HasDefaultValue(false);

    }
}
