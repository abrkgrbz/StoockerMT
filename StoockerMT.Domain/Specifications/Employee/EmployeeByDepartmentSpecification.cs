using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Specifications.Employee
{
    public class EmployeeByDepartmentSpecification : Specification<Entities.TenantDb.Employee>
    {
        private readonly int _departmentId;

        public EmployeeByDepartmentSpecification(int departmentId)
        {
            _departmentId = departmentId;
        }

        public   Expression<Func<Entities.TenantDb.Employee, bool>> ToExpression()
        {
            return employee => employee.DepartmentId == _departmentId;
        }
    }
}
