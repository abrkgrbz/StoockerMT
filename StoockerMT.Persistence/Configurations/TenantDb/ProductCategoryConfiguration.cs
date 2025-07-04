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
    public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
    {
        public void Configure(EntityTypeBuilder<ProductCategory> builder)
        {
            builder.ToTable("ProductCategories");

            builder.HasKey(pc => pc.Id);

            builder.Property(pc => pc.CategoryName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(pc => pc.Description)
                .HasMaxLength(500);

            builder.Property(pc => pc.IsActive)
                .HasDefaultValue(true);

            builder.Property(pc => pc.CreatedBy)
                .HasMaxLength(100);

            builder.Property(pc => pc.UpdatedBy)
                .HasMaxLength(100);

            builder.Property(pc => pc.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Self-referencing relationship
            builder.HasOne(pc => pc.ParentCategory)
                .WithMany(pc => pc.SubCategories)
                .HasForeignKey(pc => pc.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Products relationship
            builder.HasMany(pc => pc.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
