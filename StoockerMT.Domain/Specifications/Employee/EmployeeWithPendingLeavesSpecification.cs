using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Domain.Enums;

namespace StoockerMT.Domain.Specifications.Employee
{
    public class EmployeeWithPendingLeavesSpecification : Specification<Entities.TenantDb.Employee>
    {
        public override Expression<Func<Entities.TenantDb.Employee, bool>> ToExpression()
        {
            return employee => employee.Leaves.Any(l => l.Status == LeaveStatus.Pending);
        }
    }
}
