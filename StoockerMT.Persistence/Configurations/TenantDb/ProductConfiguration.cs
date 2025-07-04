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
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.ProductName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.ProductCode)
                .HasMaxLength(50);

            builder.HasIndex(p => p.ProductCode)
                .IsUnique()
                .HasDatabaseName("IX_Products_ProductCode")
                .HasFilter("[ProductCode] IS NOT NULL");

            builder.Property(p => p.SKU)
                .HasMaxLength(100);

            builder.HasIndex(p => p.SKU)
                .IsUnique()
                .HasDatabaseName("IX_Products_SKU")
                .HasFilter("[SKU] IS NOT NULL");

            builder.Property(p => p.Description)
                .HasMaxLength(1000);

            // Value Object: Money for UnitPrice
            builder.OwnsOne(p => p.UnitPrice, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("UnitPrice")
                    .HasColumnType("decimal(18,2)")
                    .HasDefaultValue(0);

                money.Property(m => m.Currency)
                    .HasColumnName("UnitPriceCurrency")
                    .HasMaxLength(3)
                    .HasDefaultValue("USD");
            });

            // Value Object: Money for CostPrice
            builder.OwnsOne(p => p.CostPrice, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("CostPrice")
                    .HasColumnType("decimal(18,2)")
                    .HasDefaultValue(0);

                money.Property(m => m.Currency)
                    .HasColumnName("CostPriceCurrency")
                    .HasMaxLength(3)
                    .HasDefaultValue("USD");
            });

            // Value Object: Quantity for StockQuantity
            builder.OwnsOne(p => p.StockQuantity, quantity =>
            {
                quantity.Property(q => q.Value)
                    .HasColumnName("StockQuantity")
                    .HasColumnType("decimal(18,3)")
                    .HasDefaultValue(0);

                quantity.Property(q => q.Unit)
                    .HasColumnName("StockUnit")
                    .HasMaxLength(10)
                    .HasDefaultValue("PCS");
            });

            // Value Object: Quantity for MinimumStockLevel
            builder.OwnsOne(p => p.MinimumStockLevel, quantity =>
            {
                quantity.Property(q => q.Value)
                    .HasColumnName("MinimumStockLevel")
                    .HasColumnType("decimal(18,3)")
                    .HasDefaultValue(0);

                quantity.Property(q => q.Unit)
                    .HasColumnName("MinimumStockUnit")
                    .HasMaxLength(10)
                    .HasDefaultValue("PCS");
            });

            builder.Property(p => p.Weight)
                .HasColumnType("decimal(10,3)")
                .HasDefaultValue(0);

            builder.Property(p => p.ImageUrl)
                .HasMaxLength(500);

            builder.Property(p => p.CreatedBy)
                .HasMaxLength(100);

            builder.Property(p => p.UpdatedBy)
                .HasMaxLength(100);

            builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(p => p.OrderItems)
                .WithOne(oi => oi.Product)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(p => p.InventoryMovements)
                .WithOne(im => im.Product)
                .HasForeignKey(im => im.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
