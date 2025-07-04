using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Domain.Entities.MasterDb;

namespace StoockerMT.Persistence.Configurations.MasterDb
{
    public class ModuleConfiguration : IEntityTypeConfiguration<Module>
    {
        public void Configure(EntityTypeBuilder<Module> builder)
        {
            builder.ToTable("Modules");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(m => m.Code)
                .IsUnique()
                .HasDatabaseName("IX_Modules_Code");

            builder.Property(m => m.Description)
                .HasMaxLength(1000);

            // Value Object: Money for MonthlyPrice
            builder.OwnsOne(m => m.MonthlyPrice, price =>
            {
                price.Property(p => p.Amount)
                    .HasColumnName("MonthlyPrice")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                price.Property(p => p.Currency)
                    .HasColumnName("MonthlyCurrency")
                    .HasMaxLength(3)
                    .IsRequired()
                    .HasDefaultValue("USD");
            });

            // Value Object: Money for YearlyPrice
            builder.OwnsOne(m => m.YearlyPrice, price =>
            {
                price.Property(p => p.Amount)
                    .HasColumnName("YearlyPrice")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                price.Property(p => p.Currency)
                    .HasColumnName("YearlyCurrency")
                    .HasMaxLength(3)
                    .IsRequired()
                    .HasDefaultValue("USD");
            });

            builder.Property(m => m.Version)
                .IsRequired()
                .HasMaxLength(10)
                .HasDefaultValue("1.0.0");

            builder.Property(m => m.Category)
                .HasConversion<int>();

            builder.Property(m => m.Configuration)
                .HasColumnType("nvarchar(max)");

            builder.Property(m => m.Dependencies)
                .HasColumnType("nvarchar(max)");

            builder.Property(m => m.CreatedBy)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.UpdatedBy)
                .HasMaxLength(100);

            builder.Property(m => m.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasMany(m => m.TenantSubscriptions)
                .WithOne(s => s.Module)
                .HasForeignKey(s => s.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(m => m.Permissions)
                .WithOne(p => p.Module)
                .HasForeignKey(p => p.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(m => m.Features)
                .WithOne(f => f.Module)
                .HasForeignKey(f => f.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
