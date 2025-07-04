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
    public class InventoryMovementConfiguration : IEntityTypeConfiguration<InventoryMovement>
    {
        public void Configure(EntityTypeBuilder<InventoryMovement> builder)
        {
            builder.ToTable("InventoryMovements");

            builder.HasKey(im => im.Id);

            builder.Property(im => im.Type)
                .HasConversion<int>();

            // Value Object: Quantity
            builder.OwnsOne(im => im.Quantity, quantity =>
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

            // Value Object: Money for UnitCost
            builder.OwnsOne(im => im.UnitCost, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("UnitCost")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                money.Property(m => m.Currency)
                    .HasColumnName("Currency")
                    .HasMaxLength(3)
                    .HasDefaultValue("USD");
            });

            builder.Property(im => im.Reference)
                .HasMaxLength(500);

            builder.Property(im => im.Notes)
                .HasMaxLength(1000);

            builder.Property(im => im.MovementDate)
                .IsRequired();

            builder.HasIndex(im => im.MovementDate)
                .HasDatabaseName("IX_InventoryMovements_MovementDate");

            builder.HasIndex(im => new { im.ProductId, im.MovementDate })
                .HasDatabaseName("IX_InventoryMovements_Product_Date");

            builder.Property(im => im.CreatedBy)
                .HasMaxLength(100);

            builder.Property(im => im.UpdatedBy)
                .HasMaxLength(100);

            builder.Property(im => im.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(im => im.Product)
                .WithMany(p => p.InventoryMovements)
                .HasForeignKey(im => im.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
