using StoockerMT.Domain.Entities.TenantDb;
using StoockerMT.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Specifications.Domain
{
    public class ActiveProductWithPriceRangeSpecification : Specification<Product>
    {
        private readonly Money _minPrice;
        private readonly Money _maxPrice;

        public ActiveProductWithPriceRangeSpecification(Money minPrice, Money maxPrice)
        {
            _minPrice = minPrice;
            _maxPrice = maxPrice;
        }

        public override Expression<Func<Product, bool>> ToExpression()
        {
            return product =>
                product.IsActive &&
                product.UnitPrice.Amount >= _minPrice.Amount &&
                product.UnitPrice.Amount <= _maxPrice.Amount &&
                product.UnitPrice.Currency == _minPrice.Currency;
        }
    }
}
