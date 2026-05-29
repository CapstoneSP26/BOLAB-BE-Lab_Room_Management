using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Infrastructure.Persistence.Configurations
{
    public class TabletAccountConfiguration : IEntityTypeConfiguration<TabletAccount>
    {
        public void Configure(EntityTypeBuilder<TabletAccount> builder)
        {
            builder.ToTable("TabletAccounts");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasDefaultValueSql("uuid_generate_v4()");

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Password)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasOne(x => x.LabRoom)
                .WithOne(x => x.TabletAccount)
                .HasForeignKey<TabletAccount>(x => x.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.Name)
                .IsUnique()
                .HasDatabaseName("UQ_TabletAccounts_Name");
        }
    }
}
