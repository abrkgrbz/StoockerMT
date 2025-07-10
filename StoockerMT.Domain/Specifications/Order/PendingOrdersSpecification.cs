using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Domain.Enums;

namespace StoockerMT.Domain.Specifications.Order
{
    public class PendingOrdersSpecification : Specification<Entities.TenantDb.Order>
    {
        public   Expression<Func<Entities.TenantDb.Order, bool>> ToExpression()
        {
            return order => order.Status == OrderStatus.Pending ||
                            order.Status == OrderStatus.Confirmed;
        }
    }
}
