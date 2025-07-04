using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Domain.Entities.TenantDb.Common;
using StoockerMT.Domain.Enums;
using StoockerMT.Domain.ValueObjects;

namespace StoockerMT.Domain.Entities.TenantDb
{
    public class EmployeeTimesheet : TenantBaseEntity
    {
        public int EmployeeId { get; private set; }
        public DateTime WorkDate { get; private set; }
        public TimeRange? WorkTime { get; private set; }
        public TimeRange? BreakTime { get; private set; }
        public decimal HoursWorked { get; private set; }
        public decimal OvertimeHours { get; private set; }
        public string? Notes { get; private set; }
        public TimesheetStatus Status { get; private set; } = TimesheetStatus.Draft;

        // Navigation Properties
        public virtual Employee Employee { get; set; }

        private EmployeeTimesheet() { }

        public EmployeeTimesheet(int employeeId, DateTime workDate)
        {
            EmployeeId = employeeId;
            WorkDate = workDate.Date; // Remove time component
        }

        public void RecordWorkTime(TimeSpan checkInTime, TimeSpan checkOutTime)
        {
            WorkTime = new TimeRange(checkInTime, checkOutTime);
            CalculateHours();
        }

        public void RecordBreakTime(TimeSpan breakStartTime, TimeSpan breakEndTime)
        {
            if (WorkTime == null)
                throw new InvalidOperationException("Cannot record break time before work time");

            var breakTimeRange = new TimeRange(breakStartTime, breakEndTime);

            if (!WorkTime.Overlaps(breakTimeRange))
                throw new ArgumentException("Break time must be within work hours");

            BreakTime = breakTimeRange;
            CalculateHours();
        }

        private void CalculateHours()
        {
            if (WorkTime == null)
            {
                HoursWorked = 0;
                OvertimeHours = 0;
                return;
            }

            var totalWorkHours = WorkTime.GetHours();
            var breakHours = BreakTime?.GetHours() ?? 0;

            HoursWorked = totalWorkHours - breakHours;

            // Calculate overtime (more than 8 hours)
            const decimal standardHours = 8;
            if (HoursWorked > standardHours)
            {
                OvertimeHours = HoursWorked - standardHours;
                HoursWorked = standardHours;
            }
            else
            {
                OvertimeHours = 0;
            }

            UpdateTimestamp();
        }

        public void Submit()
        {
            if (Status != TimesheetStatus.Draft)
                throw new InvalidOperationException("Only draft timesheets can be submitted");

            if (WorkTime == null)
                throw new InvalidOperationException("Cannot submit timesheet without work time");

            Status = TimesheetStatus.Submitted;
            UpdateTimestamp();
        }

        public void Approve()
        {
            if (Status != TimesheetStatus.Submitted)
                throw new InvalidOperationException("Only submitted timesheets can be approved");

            Status = TimesheetStatus.Approved;
            UpdateTimestamp();
        }

        public void Reject(string reason)
        {
            if (Status != TimesheetStatus.Submitted)
                throw new InvalidOperationException("Only submitted timesheets can be rejected");

            Status = TimesheetStatus.Rejected;
            Notes = $"Rejected: {reason}";
            UpdateTimestamp();
        }

        public decimal GetTotalHours()
        {
            return HoursWorked + OvertimeHours;
        }
    }
}
