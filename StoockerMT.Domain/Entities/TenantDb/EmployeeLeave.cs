using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Domain.Enums;
using StoockerMT.Domain.ValueObjects;

namespace StoockerMT.Domain.Entities.TenantDb.Common
{
    public class EmployeeLeave : TenantBaseEntity
    {
        public int EmployeeId { get; private set; }
        public LeaveType Type { get; private set; }
        public DateRange LeavePeriod { get; private set; }
        public int DaysRequested { get; private set; }
        public int DaysApproved { get; private set; }
        public LeaveStatus Status { get; private set; } = LeaveStatus.Pending;
        public string? Reason { get; private set; }
        public string? ApprovalNotes { get; private set; }
        public DateTime RequestDate { get; private set; }
        public DateTime? ApprovalDate { get; private set; }
        public string? ApprovedBy { get; private set; }

        // Navigation Properties
        public virtual Employee Employee { get; set; }

        private EmployeeLeave() { }

        public EmployeeLeave(int employeeId, LeaveType type, DateRange leavePeriod, string? reason = null)
        {
            EmployeeId = employeeId;
            Type = type;
            LeavePeriod = leavePeriod ?? throw new ArgumentNullException(nameof(leavePeriod));
            DaysRequested = leavePeriod.GetDays();
            Reason = reason;
            RequestDate = DateTime.UtcNow;
        }

        public void Approve(int daysApproved, string approvedBy, string? notes = null)
        {
            if (Status != LeaveStatus.Pending)
                throw new InvalidOperationException("Only pending leave requests can be approved");

            if (daysApproved > DaysRequested)
                throw new ArgumentException("Cannot approve more days than requested");

            Status = LeaveStatus.Approved;
            DaysApproved = daysApproved;
            ApprovedBy = approvedBy ?? throw new ArgumentNullException(nameof(approvedBy));
            ApprovalNotes = notes;
            ApprovalDate = DateTime.UtcNow;
            UpdateTimestamp();
        }

        public void Reject(string rejectedBy, string? notes = null)
        {
            if (Status != LeaveStatus.Pending)
                throw new InvalidOperationException("Only pending leave requests can be rejected");

            Status = LeaveStatus.Rejected;
            DaysApproved = 0;
            ApprovedBy = rejectedBy ?? throw new ArgumentNullException(nameof(rejectedBy));
            ApprovalNotes = notes;
            ApprovalDate = DateTime.UtcNow;
            UpdateTimestamp();
        }

        public void Cancel(string? reason = null)
        {
            if (Status == LeaveStatus.Cancelled)
                throw new InvalidOperationException("Leave request is already cancelled");

            Status = LeaveStatus.Cancelled;
            if (!string.IsNullOrWhiteSpace(reason))
                ApprovalNotes = $"Cancelled: {reason}";
            UpdateTimestamp();
        }

        public bool IsActive()
        {
            return Status == LeaveStatus.Approved && LeavePeriod.Contains(DateTime.UtcNow);
        }
    }
}
