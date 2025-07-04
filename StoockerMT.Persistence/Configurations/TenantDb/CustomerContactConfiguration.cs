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
    public class CustomerContactConfiguration : IEntityTypeConfiguration<CustomerContact>
    {
        public void Configure(EntityTypeBuilder<CustomerContact> builder)
        {
            builder.ToTable("CustomerContacts");

            builder.HasKey(cc => cc.Id);

            builder.Property(cc => cc.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(cc => cc.Title)
                .HasMaxLength(100);

            // Value Object: Email (optional)
            builder.OwnsOne(cc => cc.Email, email =>
            {
                email.Property(e => e.Value)
                    .HasColumnName("Email")
                    .HasMaxLength(200);
            });

            // Value Object: PhoneNumber (optional)
            builder.OwnsOne(cc => cc.PhoneNumber, phone =>
            {
                phone.Property(p => p.Value)
                    .HasColumnName("PhoneNumber")
                    .HasMaxLength(20);
            });

            builder.Property(cc => cc.IsPrimary)
                .HasDefaultValue(false);

            builder.Property(cc => cc.CreatedBy)
                .HasMaxLength(100);

            builder.Property(cc => cc.UpdatedBy)
                .HasMaxLength(100);

            builder.Property(cc => cc.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(cc => cc.Customer)
                .WithMany(c => c.Contacts)
                .HasForeignKey(cc => cc.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
