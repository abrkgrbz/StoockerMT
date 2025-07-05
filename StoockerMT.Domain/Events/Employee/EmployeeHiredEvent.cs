using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Events.Employee
{
    public class EmployeeHiredEvent : DomainEvent
    {
        public int EmployeeId { get; }
        public string EmployeeCode { get; }
        public string FullName { get; }
        public int? DepartmentId { get; }
        public DateTime HireDate { get; }

        public EmployeeHiredEvent(int employeeId, string employeeCode, string fullName,
            int? departmentId, DateTime hireDate)
        {
            EmployeeId = employeeId;
            EmployeeCode = employeeCode;
            FullName = fullName;
            DepartmentId = departmentId;
            HireDate = hireDate;
        }
    }
}
