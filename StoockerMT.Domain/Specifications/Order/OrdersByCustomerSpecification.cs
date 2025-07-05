using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Specifications.Order
{
    public class OrdersByCustomerSpecification : Specification<Entities.TenantDb.Order>
    {
        private readonly int _customerId;

        public OrdersByCustomerSpecification(int customerId)
        {
            _customerId = customerId;
        }

        public override Expression<Func<Entities.TenantDb.Order, bool>> ToExpression()
        {
            return order => order.CustomerId == _customerId;
        }
    }
}
