using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using StoockerMT.Domain.Entities.TenantDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Persistence.Configurations.TenantDb
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");

            builder.HasKey(oi => oi.Id);

            builder.Property(oi => oi.ProductName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(oi => oi.ProductCode)
                .HasMaxLength(100);

            // Value Object: Quantity
            builder.OwnsOne(oi => oi.Quantity, quantity =>
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
            builder.OwnsOne(oi => oi.UnitPrice, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("UnitPrice")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                money.Property(m => m.Currency)
                    .HasColumnName("UnitPriceCurrency")
                    .HasMaxLength(3)
                    .HasDefaultValue("USD");
            });

            // Value Object: Money for DiscountAmount
            builder.OwnsOne(oi => oi.DiscountAmount, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("DiscountAmount")
                    .HasColumnType("decimal(18,2)")
                    .HasDefaultValue(0);

                money.Property(m => m.Currency)
                    .HasColumnName("DiscountCurrency")
                    .HasMaxLength(3)
                    .HasDefaultValue("USD");
            });

            // Value Object: Money for Total
            builder.OwnsOne(oi => oi.Total, money =>
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

            builder.Property(oi => oi.CreatedBy)
                .HasMaxLength(100);

            builder.Property(oi => oi.UpdatedBy)
                .HasMaxLength(100);

            builder.Property(oi => oi.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
