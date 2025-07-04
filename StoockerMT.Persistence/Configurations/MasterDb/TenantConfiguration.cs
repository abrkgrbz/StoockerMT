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
    public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
    {
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            builder.ToTable("Tenants");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Value Object: TenantCode
            builder.OwnsOne(t => t.Code, code =>
            {
                code.Property(c => c.Value)
                    .HasColumnName("Code")
                    .IsRequired()
                    .HasMaxLength(50);

                code.HasIndex(c => c.Value)
                    .IsUnique()
                    .HasDatabaseName("IX_Tenants_Code");
            });

            builder.Property(t => t.DatabaseName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.ConnectionString)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(t => t.Description)
                .HasMaxLength(1000);

            builder.Property(t => t.Settings)
                .HasColumnType("nvarchar(max)");

            builder.Property(t => t.CreatedBy)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.UpdatedBy)
                .HasMaxLength(100);

            builder.Property(t => t.Status)
                .HasConversion<int>();

            builder.Property(t => t.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(t => t.MaxUsers)
                .HasDefaultValue(10);

            builder.Property(t => t.MaxStorageBytes)
                .HasDefaultValue(1073741824L);

            // Relationships
            builder.HasMany(t => t.ModuleSubscriptions)
                .WithOne(s => s.Tenant)
                .HasForeignKey(s => s.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.Users)
                .WithOne(u => u.Tenant)
                .HasForeignKey(u => u.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.Invoices)
                .WithOne(i => i.Tenant)
                .HasForeignKey(i => i.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 
 
