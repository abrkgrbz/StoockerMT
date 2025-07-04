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
    public class EmployeeTimesheetConfiguration : IEntityTypeConfiguration<EmployeeTimesheet>
    {
        public void Configure(EntityTypeBuilder<EmployeeTimesheet> builder)
        {
            builder.ToTable("EmployeeTimesheets");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.WorkDate)
                .IsRequired();

            builder.HasIndex(t => new { t.EmployeeId, t.WorkDate })
                .IsUnique()
                .HasDatabaseName("IX_EmployeeTimesheets_Employee_Date");

            // Value Object: TimeRange for WorkTime (optional)
            builder.OwnsOne(t => t.WorkTime, time =>
            {
                time.Property(tr => tr.StartTime)
                    .HasColumnName("CheckInTime")
                    .HasColumnType("time");

                time.Property(tr => tr.EndTime)
                    .HasColumnName("CheckOutTime")
                    .HasColumnType("time");
            });

            // Value Object: TimeRange for BreakTime (optional)
            builder.OwnsOne(t => t.BreakTime, time =>
            {
                time.Property(tr => tr.StartTime)
                    .HasColumnName("BreakStartTime")
                    .HasColumnType("time");

                time.Property(tr => tr.EndTime)
                    .HasColumnName("BreakEndTime")
                    .HasColumnType("time");
            });

            builder.Property(t => t.HoursWorked)
                .HasColumnType("decimal(5,2)")
                .HasDefaultValue(0);

            builder.Property(t => t.OvertimeHours)
                .HasColumnType("decimal(5,2)")
                .HasDefaultValue(0);

            builder.Property(t => t.Notes)
                .HasMaxLength(1000);

            builder.Property(t => t.Status)
                .HasConversion<int>();

            builder.Property(t => t.CreatedBy)
                .HasMaxLength(100);

            builder.Property(t => t.UpdatedBy)
                .HasMaxLength(100);

            builder.Property(t => t.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(t => t.Employee)
                .WithMany(e => e.Timesheets)
                .HasForeignKey(t => t.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
