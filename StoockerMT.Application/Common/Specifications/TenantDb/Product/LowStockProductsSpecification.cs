using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Specifications.TenantDb.Product
{
    public class LowStockProductsSpecification : BaseSpecification<Domain.Entities.TenantDb.Product>
    {
        public LowStockProductsSpecification()
            : base(p => p.IsActive &&
                        p.StockQuantity.Value <= p.MinimumStockLevel.Value)
        {
            ApplyOrderBy(p => p.StockQuantity.Value);
        }
    }
}
