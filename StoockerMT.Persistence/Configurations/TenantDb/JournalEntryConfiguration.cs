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
    public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
    {
        public void Configure(EntityTypeBuilder<JournalEntry> builder)
        {
            builder.ToTable("JournalEntries");

            builder.HasKey(j => j.Id);

            builder.Property(j => j.EntryNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(j => j.EntryNumber)
                .IsUnique()
                .HasDatabaseName("IX_JournalEntries_EntryNumber");

            builder.Property(j => j.EntryDate)
                .IsRequired();

            builder.HasIndex(j => j.EntryDate)
                .HasDatabaseName("IX_JournalEntries_EntryDate");

            builder.Property(j => j.Description)
                .HasMaxLength(500);

            builder.Property(j => j.Reference)
                .HasMaxLength(200);

            // Value Object: Money for TotalDebit
            builder.OwnsOne(j => j.TotalDebit, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("TotalDebit")
                    .HasColumnType("decimal(18,2)")
                    .HasDefaultValue(0);

                money.Property(m => m.Currency)
                    .HasColumnName("DebitCurrency")
                    .HasMaxLength(3)
                    .HasDefaultValue("USD");
            });

            // Value Object: Money for TotalCredit
            builder.OwnsOne(j => j.TotalCredit, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("TotalCredit")
                    .HasColumnType("decimal(18,2)")
                    .HasDefaultValue(0);

                money.Property(m => m.Currency)
                    .HasColumnName("CreditCurrency")
                    .HasMaxLength(3)
                    .HasDefaultValue("USD");
            });

            builder.Property(j => j.Status)
                .HasConversion<int>();

            builder.Property(j => j.CreatedBy)
                .HasMaxLength(100);

            builder.Property(j => j.UpdatedBy)
                .HasMaxLength(100);

            builder.Property(j => j.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasMany(j => j.Lines)
                .WithOne(l => l.JournalEntry)
                .HasForeignKey(l => l.JournalEntryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
