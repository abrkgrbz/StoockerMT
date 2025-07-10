using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Specifications.TenantDb.Product
{
    public class BestSellingProductsSpecification : BaseSpecification<Domain.Entities.TenantDb.Product>
    {
        public BestSellingProductsSpecification(DateTime fromDate, int topN = 10)
        {
            Criteria = p => p.IsActive &&
                            p.OrderItems.Any(oi => oi.Order.OrderDate >= fromDate);

            AddInclude(p => p.OrderItems);
            AddInclude("OrderItems.Order");

            // Note: Ordering by aggregate requires post-processing
            // This would be better handled in a service layer
            ApplyOrderByDescending(p => p.OrderItems.Count);

            if (topN > 0)
            {
                ApplyPaging(0, topN);
            }

            ApplySplitQuery();
        }
    }
}
