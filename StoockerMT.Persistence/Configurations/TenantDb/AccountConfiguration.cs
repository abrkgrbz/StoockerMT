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
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Accounts");

            builder.HasKey(a => a.Id);

            // Value Object: AccountCode
            builder.OwnsOne(a => a.AccountCode, code =>
            {
                code.Property(c => c.Value)
                    .HasColumnName("AccountCode")
                    .IsRequired()
                    .HasMaxLength(50);

                code.HasIndex(c => c.Value)
                    .IsUnique()
                    .HasDatabaseName("IX_Accounts_AccountCode");
            });

            builder.Property(a => a.AccountName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(a => a.Type)
                .HasConversion<int>();

            // Value Object: Money for Balance
            builder.OwnsOne(a => a.Balance, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("Balance")
                    .HasColumnType("decimal(18,2)")
                    .HasDefaultValue(0);

                money.Property(m => m.Currency)
                    .HasColumnName("BalanceCurrency")
                    .HasMaxLength(3)
                    .HasDefaultValue("USD");
            });

            builder.Property(a => a.Description)
                .HasMaxLength(1000);

            builder.Property(a => a.CreatedBy)
                .HasMaxLength(100);

            builder.Property(a => a.UpdatedBy)
                .HasMaxLength(100);

            builder.Property(a => a.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Self-referencing relationship
            builder.HasOne(a => a.ParentAccount)
                .WithMany(a => a.SubAccounts)
                .HasForeignKey(a => a.ParentAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // Journal entries relationship
            builder.HasMany(a => a.JournalEntryLines)
                .WithOne(j => j.Account)
                .HasForeignKey(j => j.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
