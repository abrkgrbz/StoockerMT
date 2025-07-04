using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoockerMT.Domain.Entities.MasterDb;

namespace StoockerMT.Persistence.Configurations.MasterDb
{
    public class TenantUserConfiguration : IEntityTypeConfiguration<TenantUser>
    {
        public void Configure(EntityTypeBuilder<TenantUser> builder)
        {
            builder.ToTable("TenantUsers");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100);

            // Value Object: Email
            builder.OwnsOne(u => u.Email, email =>
            {
                email.Property(e => e.Value)
                    .HasColumnName("Email")
                    .IsRequired()
                    .HasMaxLength(200);

                email.HasIndex(e => e.Value)
                    .IsUnique()
                    .HasDatabaseName("IX_TenantUsers_Email");
            });

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            // Value Object: PhoneNumber (optional)
            builder.OwnsOne(u => u.PhoneNumber, phone =>
            {
                phone.Property(p => p.Value)
                    .HasColumnName("PhoneNumber")
                    .HasMaxLength(20);
            });

            builder.Property(u => u.CreatedBy)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.UpdatedBy)
                .HasMaxLength(100);

            builder.Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(u => u.Tenant)
                .WithMany(t => t.Users)
                .HasForeignKey(u => u.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Permissions)
                .WithOne(p => p.TenantUser)
                .HasForeignKey(p => p.TenantUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
