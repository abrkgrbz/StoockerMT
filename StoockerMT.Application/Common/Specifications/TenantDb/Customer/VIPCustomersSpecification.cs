using StoockerMT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Specifications.TenantDb.Customer
{
    public class VIPCustomersSpecification : BaseSpecification<Domain.Entities.TenantDb.Customer>
    {
        public VIPCustomersSpecification(decimal minimumOrderTotal, string currency = "USD")
        {
            Criteria = c => c.Orders
                .Where(o => o.Status == OrderStatus.Delivered)
                .Sum(o => o.Total.Amount) >= minimumOrderTotal;

            AddInclude(c => c.Orders);
            ApplyOrderByDescending(c => c.Orders.Sum(o => o.Total.Amount));
        }
    }
}
