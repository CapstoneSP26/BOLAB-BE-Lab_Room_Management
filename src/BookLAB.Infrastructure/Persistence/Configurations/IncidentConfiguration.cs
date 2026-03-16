using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookLAB.Infrastructure.Persistence.Configurations;

public class IncidentConfiguration : IEntityTypeConfiguration<Incident>
{
    public void Configure(EntityTypeBuilder<Incident> builder)
    {
        builder.ToTable("Incidents");
        builder.HasKey(incident => incident.Id);

        builder.Property(incident => incident.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(incident => incident.Description)
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(incident => incident.Severity)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(incident => incident.Environment)
            .HasMaxLength(200)
            .IsRequired(false);

        builder.Property(incident => incident.StepsToReproduce)
            .HasColumnType("text[]")
            .IsRequired();

        builder.Property(incident => incident.ExpectedResult)
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(incident => incident.ActualResult)
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(incident => incident.AttachmentUrl)
            .HasMaxLength(2048)
            .IsRequired(false);

        builder.Property(incident => incident.ReportedBy)
            .IsRequired();

        builder.Property(incident => incident.CreatedAt)
            .IsRequired();

        builder.Property(incident => incident.CreatedBy)
            .IsRequired();

        builder.HasOne(incident => incident.ReportedByUser)
            .WithMany()
            .HasForeignKey(incident => incident.ReportedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(incident => incident.Severity);
        builder.HasIndex(incident => incident.ReportedBy);
        builder.HasIndex(incident => incident.CreatedAt);
    }
}