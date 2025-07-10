using StoockerMT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Specifications.TenantDb.Customer
{
    public class CustomersWithCreditExceededSpecification : BaseSpecification<Domain.Entities.TenantDb.Customer>
    {
        public CustomersWithCreditExceededSpecification()
        {
            Criteria = c => c.Status == CustomerStatus.Active &&
                            c.CreditLimit != null &&
                            c.Orders.Where(o =>
                                    o.Status != OrderStatus.Cancelled &&
                                    o.Status != OrderStatus.Delivered)
                                .Sum(o => o.Total.Amount) > c.CreditLimit.Amount;

            AddInclude(c => c.Orders);
        }
    }
}
