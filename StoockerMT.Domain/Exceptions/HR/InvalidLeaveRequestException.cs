using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Exceptions.HR
{
    public class InvalidLeaveRequestException : DomainException
    {
        public int EmployeeId { get; }
        public string Reason { get; }

        public InvalidLeaveRequestException(int employeeId, string reason)
            : base("INVALID_LEAVE_REQUEST", $"Invalid leave request for employee '{employeeId}': {reason}")
        {
            EmployeeId = employeeId;
            Reason = reason;
        }
    }
}
