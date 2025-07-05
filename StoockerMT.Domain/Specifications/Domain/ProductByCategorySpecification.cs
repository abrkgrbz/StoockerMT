using StoockerMT.Domain.Entities.TenantDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Specifications.Domain
{
    public class ProductByCategorySpecification : Specification<Product>
    {
        private readonly int _categoryId;

        public ProductByCategorySpecification(int categoryId)
        {
            _categoryId = categoryId;
        }

        public override Expression<Func<Product, bool>> ToExpression()
        {
            return product => product.CategoryId == _categoryId;
        }
    }
}
