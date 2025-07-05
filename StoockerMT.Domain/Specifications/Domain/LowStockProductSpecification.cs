using StoockerMT.Domain.Entities.TenantDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Specifications.Domain
{
    public class LowStockProductSpecification : Specification<Product>
    {
        public override Expression<Func<Product, bool>> ToExpression()
        {
            return product => product.StockQuantity.Value <= product.MinimumStockLevel.Value;
        }
    }
}
