using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Specifications.TenantDb.Product
{
    public class ProductsByPriceRangeSpecification : BaseSpecification<Domain.Entities.TenantDb.Product>
    {
        public ProductsByPriceRangeSpecification(decimal minPrice, decimal maxPrice, string currency = "USD")
        {
            Criteria = p => p.IsActive &&
                            p.UnitPrice.Currency == currency &&
                            p.UnitPrice.Amount >= minPrice &&
                            p.UnitPrice.Amount <= maxPrice;

            ApplyOrderBy(p => p.UnitPrice.Amount);
        }
    }
}
