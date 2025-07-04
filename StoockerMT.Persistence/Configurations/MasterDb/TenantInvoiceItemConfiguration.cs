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
    public class TenantInvoiceItemConfiguration : IEntityTypeConfiguration<TenantInvoiceItem>
    {
        public void Configure(EntityTypeBuilder<TenantInvoiceItem> builder)
        {
            builder.ToTable("TenantInvoiceItems");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.Description)
                .IsRequired()
                .HasMaxLength(200);

            // Value Object: Quantity
            builder.OwnsOne(i => i.Quantity, quantity =>
            {
                quantity.Property(q => q.Value)
                    .HasColumnName("Quantity")
                    .HasColumnType("decimal(18,3)")
                    .IsRequired();

                quantity.Property(q => q.Unit)
                    .HasColumnName("Unit")
                    .HasMaxLength(10)
                    .HasDefaultValue("PCS");
            });

            // Value Object: Money for UnitPrice
            builder.OwnsOne(i => i.UnitPrice, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("UnitPrice")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                money.Property(m => m.Currency)
                    .HasColumnName("Currency")
                    .HasMaxLength(3)
                    .HasDefaultValue("USD");
            });

            // Value Object: Money for Total
            builder.OwnsOne(i => i.Total, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("Total")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                money.Property(m => m.Currency)
                    .HasColumnName("TotalCurrency")
                    .HasMaxLength(3)
                    .HasDefaultValue("USD");
            });

            builder.Property(i => i.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(i => i.Invoice)
                .WithMany(inv => inv.Items)
                .HasForeignKey(i => i.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(i => i.Module)
                .WithMany()
                .HasForeignKey(i => i.ModuleId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
