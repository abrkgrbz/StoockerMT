using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using StoockerMT.Domain.Entities.MasterDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Persistence.c.MasterDb
{
    public class ModulePermissionConfiguration : IEntityTypeConfiguration<ModulePermission>
    {
        public void Configure(EntityTypeBuilder<ModulePermission> builder)
        {
            builder.ToTable("ModulePermissions");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(p => new { p.ModuleId, p.Code })
                .IsUnique()
                .HasDatabaseName("IX_ModulePermissions_ModuleId_Code");

            builder.Property(p => p.Description)
                .HasMaxLength(500);

            builder.Property(p => p.Type)
                .HasConversion<int>();

            builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(p => p.Module)
                .WithMany(m => m.Permissions)
                .HasForeignKey(p => p.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.UserPermissions)
                .WithOne(up => up.Permission)
                .HasForeignKey(up => up.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
