using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using StoockerMT.Domain.Entities.TenantDb.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Persistence.Configurations.TenantDb
{
    public class EmployeeLeaveConfiguration : IEntityTypeConfiguration<EmployeeLeave>
    {
        public void Configure(EntityTypeBuilder<EmployeeLeave> builder)
        {
            builder.ToTable("EmployeeLeaves");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.Type)
                .HasConversion<int>();

            // Value Object: DateRange for LeavePeriod
            builder.OwnsOne(l => l.LeavePeriod, period =>
            {
                period.Property(p => p.StartDate)
                    .HasColumnName("StartDate")
                    .IsRequired();

                period.Property(p => p.EndDate)
                    .HasColumnName("EndDate")
                    .IsRequired();

                period.HasIndex(p => new { p.StartDate, p.EndDate })
                    .HasDatabaseName("IX_EmployeeLeaves_Period");
            });

            builder.Property(l => l.DaysRequested)
                .IsRequired();

            builder.Property(l => l.DaysApproved)
                .HasDefaultValue(0);

            builder.Property(l => l.Status)
                .HasConversion<int>();

            builder.Property(l => l.Reason)
                .HasMaxLength(1000);

            builder.Property(l => l.ApprovalNotes)
                .HasMaxLength(1000);

            builder.Property(l => l.RequestDate)
                .IsRequired();

            builder.Property(l => l.ApprovedBy)
                .HasMaxLength(100);

            builder.Property(l => l.CreatedBy)
                .HasMaxLength(100);

            builder.Property(l => l.UpdatedBy)
                .HasMaxLength(100);

            builder.Property(l => l.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(l => l.Employee)
                .WithMany(e => e.Leaves)
                .HasForeignKey(l => l.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
