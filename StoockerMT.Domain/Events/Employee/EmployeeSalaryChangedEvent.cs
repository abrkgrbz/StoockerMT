using StoockerMT.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Events.Employee
{
    public class EmployeeSalaryChangedEvent : DomainEvent
    {
        public int EmployeeId { get; }
        public Money OldSalary { get; }
        public Money NewSalary { get; }
        public string ChangedBy { get; }

        public EmployeeSalaryChangedEvent(int employeeId, Money oldSalary, Money newSalary, string changedBy)
        {
            EmployeeId = employeeId;
            OldSalary = oldSalary;
            NewSalary = newSalary;
            ChangedBy = changedBy;
        }
    }
}
