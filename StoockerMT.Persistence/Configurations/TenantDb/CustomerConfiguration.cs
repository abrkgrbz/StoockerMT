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
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.CustomerName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.CustomerCode)
                .HasMaxLength(50);

            builder.HasIndex(c => c.CustomerCode)
                .IsUnique()
                .HasDatabaseName("IX_Customers_CustomerCode")
                .HasFilter("[CustomerCode] IS NOT NULL");

            builder.Property(c => c.ContactPerson)
                .HasMaxLength(100);

            // Value Object: Email (optional)
            builder.OwnsOne(c => c.Email, email =>
            {
                email.Property(e => e.Value)
                    .HasColumnName("Email")
                    .HasMaxLength(200);

                email.HasIndex(e => e.Value)
                    .HasDatabaseName("IX_Customers_Email")
                    .HasFilter("[Email] IS NOT NULL");
            });

            // Value Object: PhoneNumber (optional)
            builder.OwnsOne(c => c.PhoneNumber, phone =>
            {
                phone.Property(p => p.Value)
                    .HasColumnName("PhoneNumber")
                    .HasMaxLength(20);
            });

            // Value Object: Address (optional)
            builder.OwnsOne(c => c.Address, address =>
            {
                address.Property(a => a.Street).HasColumnName("Street").HasMaxLength(500);
                address.Property(a => a.City).HasColumnName("City").HasMaxLength(100);
                address.Property(a => a.State).HasColumnName("State").HasMaxLength(100);
                address.Property(a => a.Country).HasColumnName("Country").HasMaxLength(100);
                address.Property(a => a.ZipCode).HasColumnName("ZipCode").HasMaxLength(20);
            });

            builder.Property(c => c.Type)
                .HasConversion<int>();

            builder.Property(c => c.Status)
                .HasConversion<int>();

            // Value Object: Money for CreditLimit
            builder.OwnsOne(c => c.CreditLimit, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("CreditLimit")
                    .HasColumnType("decimal(18,2)")
                    .HasDefaultValue(0);

                money.Property(m => m.Currency)
                    .HasColumnName("CreditLimitCurrency")
                    .HasMaxLength(3)
                    .HasDefaultValue("USD");
            });

            builder.Property(c => c.Notes)
                .HasMaxLength(1000);

            builder.Property(c => c.CreatedBy)
                .HasMaxLength(100);

            builder.Property(c => c.UpdatedBy)
                .HasMaxLength(100);

            builder.Property(c => c.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasMany(c => c.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Contacts)
                .WithOne(cc => cc.Customer)
                .HasForeignKey(cc => cc.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
