using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Domain.Enums;

namespace StoockerMT.Domain.Specifications.Tenant
{
    public class ActiveTenantSpecification : Specification<Entities.MasterDb.Tenant>
    {
        public   Expression<Func<Entities.MasterDb.Tenant, bool>> ToExpression()
        {
            return tenant => tenant.Status == TenantStatus.Active;
        }
    }
}
