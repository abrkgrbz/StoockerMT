using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Specifications.Order
{
    public class OrdersInDateRangeSpecification : Specification<Entities.TenantDb.Order>
    {
        private readonly DateTime _startDate;
        private readonly DateTime _endDate;

        public OrdersInDateRangeSpecification(DateTime startDate, DateTime endDate)
        {
            _startDate = startDate;
            _endDate = endDate;
        }

        public   Expression<Func<Entities.TenantDb.Order, bool>> ToExpression()
        {
            return order => order.OrderDate >= _startDate && order.OrderDate <= _endDate;
        }
    }
}
