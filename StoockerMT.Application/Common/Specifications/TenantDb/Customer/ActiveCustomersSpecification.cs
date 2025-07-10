using StoockerMT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Specifications.TenantDb.Customer
{
    public class ActiveCustomersSpecification : BaseSpecification<Domain.Entities.TenantDb.Customer>
    {
        public ActiveCustomersSpecification()
            : base(c => c.Status == CustomerStatus.Active && !c.IsDeleted)
        {
            ApplyOrderBy(c => c.CustomerName);
        }
    }
}
