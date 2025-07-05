using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Events.Employee
{
    public class EmployeeTerminatedEvent : DomainEvent
    {
        public int EmployeeId { get; }
        public DateTime TerminationDate { get; }
        public string Reason { get; }

        public EmployeeTerminatedEvent(int employeeId, DateTime terminationDate, string reason)
        {
            EmployeeId = employeeId;
            TerminationDate = terminationDate;
            Reason = reason;
        }
    }

}
