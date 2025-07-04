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
    public class TenantUserPermissionConfiguration : IEntityTypeConfiguration<TenantUserPermission>
    {
        public void Configure(EntityTypeBuilder<TenantUserPermission> builder)
        {
            builder.ToTable("TenantUserPermissions");

            builder.HasKey(p => p.Id);

            builder.HasIndex(p => new { p.TenantUserId, p.PermissionId })
                .IsUnique()
                .HasDatabaseName("IX_TenantUserPermissions_User_Permission");

            builder.Property(p => p.GrantedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(p => p.GrantedBy)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(p => p.TenantUser)
                .WithMany(u => u.Permissions)
                .HasForeignKey(p => p.TenantUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Permission)
                .WithMany(mp => mp.UserPermissions)
                .HasForeignKey(p => p.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
