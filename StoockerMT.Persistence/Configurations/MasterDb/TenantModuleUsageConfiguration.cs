using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using StoockerMT.Domain.Entities.MasterDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Persistence.Configurations.MasterDb
{
    public class TenantModuleUsageConfiguration : IEntityTypeConfiguration<TenantModuleUsage>
    {
        public void Configure(EntityTypeBuilder<TenantModuleUsage> builder)
        {
            builder.ToTable("TenantModuleUsages");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Feature)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.UsageCount)
                .IsRequired();

            builder.Property(u => u.UsageDate)
                .IsRequired();

            builder.HasIndex(u => u.UsageDate)
                .HasDatabaseName("IX_TenantModuleUsages_UsageDate");

            builder.HasIndex(u => new { u.SubscriptionId, u.UsageDate })
                .HasDatabaseName("IX_TenantModuleUsages_Subscription_Date");

            builder.Property(u => u.MetaData)
                .HasMaxLength(500);

            builder.Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(u => u.Subscription)
                .WithMany(s => s.UsageRecords)
                .HasForeignKey(u => u.SubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
