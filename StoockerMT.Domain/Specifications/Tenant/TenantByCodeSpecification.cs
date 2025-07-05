using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Specifications.Tenant
{
    public class TenantByCodeSpecification : Specification<Entities.MasterDb.Tenant>
    {
        private readonly string _code;

        public TenantByCodeSpecification(string code)
        {
            _code = code;
        }

        public override Expression<Func<Entities.MasterDb.Tenant, bool>> ToExpression()
        {
            return tenant => tenant.Code.Value == _code;
        }
    }
}
