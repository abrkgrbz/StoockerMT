using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Specifications.TenantDb.Customer
{
    public class CustomersWithOrdersSpecification : BaseSpecification<Domain.Entities.TenantDb.Customer>
    {
        public CustomersWithOrdersSpecification()
            : base(c => c.Orders.Any())
        {
            AddInclude(c => c.Orders);
            ApplySplitQuery(); 
        }
    }
}
