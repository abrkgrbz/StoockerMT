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
    public class TenantInvoiceConfiguration : IEntityTypeConfiguration<TenantInvoice>
    {
        public void Configure(EntityTypeBuilder<TenantInvoice> builder)
        {
            builder.ToTable("TenantInvoices");

            builder.HasKey(i => i.Id);

            // Value Object: InvoiceNumber
            builder.OwnsOne(i => i.InvoiceNumber, number =>
            {
                number.Property(n => n.Value)
                    .HasColumnName("InvoiceNumber")
                    .IsRequired()
                    .HasMaxLength(50);

                number.HasIndex(n => n.Value)
                    .IsUnique()
                    .HasDatabaseName("IX_TenantInvoices_InvoiceNumber");
            });

            // Value Object: DateRange for BillingPeriod
            builder.OwnsOne(i => i.BillingPeriod, period =>
            {
                period.Property(p => p.StartDate)
                    .HasColumnName("BillingStartDate")
                    .IsRequired();

                period.Property(p => p.EndDate)
                    .HasColumnName("BillingEndDate")
                    .IsRequired();
            });

            builder.Property(i => i.DueDate)
                .IsRequired();

            builder.HasIndex(i => i.DueDate)
                .HasDatabaseName("IX_TenantInvoices_DueDate");

            // Value Objects: Money fields
            builder.OwnsOne(i => i.SubTotal, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("SubTotal")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                money.Property(m => m.Currency)
                    .HasColumnName("SubTotalCurrency")
                    .HasMaxLength(3)
                    .IsRequired()
                    .HasDefaultValue("USD");
            });

            builder.OwnsOne(i => i.TaxAmount, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("TaxAmount")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                money.Property(m => m.Currency)
                    .HasColumnName("TaxCurrency")
                    .HasMaxLength(3)
                    .IsRequired()
                    .HasDefaultValue("USD");
            });

            builder.OwnsOne(i => i.Total, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("Total")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                money.Property(m => m.Currency)
                    .HasColumnName("TotalCurrency")
                    .HasMaxLength(3)
                    .IsRequired()
                    .HasDefaultValue("USD");
            });

            builder.Property(i => i.Status)
                .HasConversion<int>();

            builder.Property(i => i.Notes)
                .HasMaxLength(1000);

            builder.Property(i => i.PaymentReference)
                .HasMaxLength(100);

            builder.HasIndex(i => new { i.TenantId, i.Status })
                .HasDatabaseName("IX_TenantInvoices_TenantStatus");

            // Navigation Properties
            builder.HasOne(i => i.Tenant)
                .WithMany(t => t.Invoices)
                .HasForeignKey(i => i.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(i => i.Items)
                .WithOne(item => item.Invoice)
                .HasForeignKey(item => item.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
