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
    public class TenantModuleSubscriptionConfiguration : IEntityTypeConfiguration<TenantModuleSubscription>
    {
        public void Configure(EntityTypeBuilder<TenantModuleSubscription> builder)
        {
            builder.ToTable("TenantModuleSubscriptions");

            builder.HasKey(s => s.Id);

            builder.HasIndex(s => new { s.TenantId, s.ModuleId })
                .HasDatabaseName("IX_TenantModuleSubscriptions_TenantId_ModuleId");

            // Value Object: DateRange for SubscriptionPeriod
            builder.OwnsOne(s => s.SubscriptionPeriod, period =>
            {
                period.Property(p => p.StartDate)
                    .HasColumnName("SubscriptionStartDate")
                    .IsRequired();

                period.Property(p => p.EndDate)
                    .HasColumnName("SubscriptionEndDate")
                    .IsRequired();

                period.HasIndex(p => p.EndDate)
                    .HasDatabaseName("IX_TenantModuleSubscriptions_SubscriptionEndDate");
            });

            builder.Property(s => s.SubscriptionType)
                .HasConversion<int>();

            builder.Property(s => s.Status)
                .HasConversion<int>();

            // Value Object: Money for Amount
            builder.OwnsOne(s => s.Amount, amount =>
            {
                amount.Property(a => a.Amount)
                    .HasColumnName("Amount")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                amount.Property(a => a.Currency)
                    .HasColumnName("Currency")
                    .HasMaxLength(3)
                    .IsRequired()
                    .HasDefaultValue("USD");
            });

            // Value Object: Money for DiscountAmount (optional)
            builder.OwnsOne(s => s.DiscountAmount, discount =>
            {
                discount.Property(d => d.Amount)
                    .HasColumnName("DiscountAmount")
                    .HasColumnType("decimal(18,2)");

                discount.Property(d => d.Currency)
                    .HasColumnName("DiscountCurrency")
                    .HasMaxLength(3);
            });

            builder.Property(s => s.AutoRenew)
                .HasDefaultValue(true);

            builder.Property(s => s.CreatedBy)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.UpdatedBy)
                .HasMaxLength(100);

            builder.Property(s => s.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(s => s.Tenant)
                .WithMany(t => t.ModuleSubscriptions)
                .HasForeignKey(s => s.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.Module)
                .WithMany(m => m.TenantSubscriptions)
                .HasForeignKey(s => s.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.UsageRecords)
                .WithOne(u => u.Subscription)
                .HasForeignKey(u => u.SubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
