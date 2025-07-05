using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Exceptions.Module
{
    public class ModuleSubscriptionExpiredException : DomainException
    {
        public int TenantId { get; }
        public int ModuleId { get; }
        public DateTime ExpiredAt { get; }

        public ModuleSubscriptionExpiredException(int tenantId, int moduleId, DateTime expiredAt)
            : base("MODULE_SUBSCRIPTION_EXPIRED",
                $"Subscription for module '{moduleId}' expired on {expiredAt:yyyy-MM-dd} for tenant '{tenantId}'.")
        {
            TenantId = tenantId;
            ModuleId = moduleId;
            ExpiredAt = expiredAt;
        }
    }
}
