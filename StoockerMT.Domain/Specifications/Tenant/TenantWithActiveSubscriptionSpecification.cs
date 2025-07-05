using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Domain.Enums;

namespace StoockerMT.Domain.Specifications.Tenant
{
    public class TenantWithActiveSubscriptionSpecification : Specification<Entities.MasterDb.Tenant>
    {
        private readonly int _moduleId;

        public TenantWithActiveSubscriptionSpecification(int moduleId)
        {
            _moduleId = moduleId;
        }

        public override Expression<Func<Entities.MasterDb.Tenant, bool>> ToExpression()
        {
            return tenant => tenant.ModuleSubscriptions.Any(s =>
                s.ModuleId == _moduleId &&
                s.Status == SubscriptionStatus.Active &&
                s.SubscriptionPeriod.Contains(DateTime.UtcNow));
        }
    }
}
