using StoockerMT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Exceptions.HR
{
    public class InsufficientLeaveBalanceException : DomainException
    {
        public int EmployeeId { get; }
        public LeaveType LeaveType { get; }
        public int RequestedDays { get; }
        public int AvailableDays { get; }

        public InsufficientLeaveBalanceException(int employeeId, LeaveType leaveType,
            int requestedDays, int availableDays)
            : base("INSUFFICIENT_LEAVE_BALANCE",
                $"Employee '{employeeId}' has insufficient {leaveType} leave balance. " +
                $"Requested: {requestedDays}, Available: {availableDays}")
        {
            EmployeeId = employeeId;
            LeaveType = leaveType;
            RequestedDays = requestedDays;
            AvailableDays = availableDays;
        }
    }
}
