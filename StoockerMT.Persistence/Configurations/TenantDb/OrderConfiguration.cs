using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Domain.Entities.TenantDb;

namespace StoockerMT.Persistence.Configurations.TenantDb
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(o => o.Id);

            // Value Object: OrderNumber
            builder.OwnsOne(o => o.OrderNumber, number =>
            {
                number.Property(n => n.Value)
                    .HasColumnName("OrderNumber")
                    .IsRequired()
                    .HasMaxLength(50);

                number.HasIndex(n => n.Value)
                    .IsUnique()
                    .HasDatabaseName("IX_Orders_OrderNumber");
            });

            builder.HasIndex(o => o.OrderDate)
                .HasDatabaseName("IX_Orders_OrderDate");

            // Value Objects: Money fields
            builder.OwnsOne(o => o.SubTotal, ConfigureMoney("SubTotal"));
            builder.OwnsOne(o => o.TaxAmount, ConfigureMoney("TaxAmount"));
            builder.OwnsOne(o => o.DiscountAmount, ConfigureMoney("DiscountAmount"));
            builder.OwnsOne(o => o.ShippingAmount, ConfigureMoney("ShippingAmount"));
            builder.OwnsOne(o => o.Total, ConfigureMoney("Total"));

            builder.Property(o => o.Status)
                .HasConversion<int>();

            builder.Property(o => o.Notes)
                .HasMaxLength(1000);

            // Value Object: Address for ShippingAddress (optional)
            builder.OwnsOne(o => o.ShippingAddress, address =>
            {
                address.Property(a => a.Street).HasColumnName("ShippingStreet").HasMaxLength(500);
                address.Property(a => a.City).HasColumnName("ShippingCity").HasMaxLength(100);
                address.Property(a => a.State).HasColumnName("ShippingState").HasMaxLength(100);
                address.Property(a => a.Country).HasColumnName("ShippingCountry").HasMaxLength(100);
                address.Property(a => a.ZipCode).HasColumnName("ShippingZipCode").HasMaxLength(20);
            });

            builder.Property(o => o.CreatedBy)
                .HasMaxLength(100);

            builder.Property(o => o.UpdatedBy)
                .HasMaxLength(100);

            builder.Property(o => o.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(o => o.Items)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private static Action<OwnedNavigationBuilder<Order, Domain.ValueObjects.Money>> ConfigureMoney(string columnPrefix)
        {
            return money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName(columnPrefix)
                    .HasColumnType("decimal(18,2)")
                    .HasDefaultValue(0);

                money.Property(m => m.Currency)
                    .HasColumnName($"{columnPrefix}Currency")
                    .HasMaxLength(3)
                    .HasDefaultValue("USD");
            };
        }
    }
}
