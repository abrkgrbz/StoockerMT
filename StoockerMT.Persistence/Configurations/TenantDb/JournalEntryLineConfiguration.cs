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
    public class JournalEntryLineConfiguration : IEntityTypeConfiguration<JournalEntryLine>
    {
        public void Configure(EntityTypeBuilder<JournalEntryLine> builder)
        {
            builder.ToTable("JournalEntryLines");

            builder.HasKey(jl => jl.Id);

            builder.Property(jl => jl.Description)
                .HasMaxLength(500);

            // Value Object: Money for DebitAmount
            builder.OwnsOne(jl => jl.DebitAmount, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("DebitAmount")
                    .HasColumnType("decimal(18,2)")
                    .HasDefaultValue(0);

                money.Property(m => m.Currency)
                    .HasColumnName("DebitCurrency")
                    .HasMaxLength(3)
                    .HasDefaultValue("USD");
            });

            // Value Object: Money for CreditAmount
            builder.OwnsOne(jl => jl.CreditAmount, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("CreditAmount")
                    .HasColumnType("decimal(18,2)")
                    .HasDefaultValue(0);

                money.Property(m => m.Currency)
                    .HasColumnName("CreditCurrency")
                    .HasMaxLength(3)
                    .HasDefaultValue("USD");
            });

            builder.Property(jl => jl.CreatedBy)
                .HasMaxLength(100);

            builder.Property(jl => jl.UpdatedBy)
                .HasMaxLength(100);

            builder.Property(jl => jl.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(jl => jl.JournalEntry)
                .WithMany(j => j.Lines)
                .HasForeignKey(jl => jl.JournalEntryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(jl => jl.Account)
                .WithMany(a => a.JournalEntryLines)
                .HasForeignKey(jl => jl.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
