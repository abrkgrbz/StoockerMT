using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Events.Tenant
{
    public class TenantCreatedEvent : DomainEvent
    {
        public int TenantId { get; }
        public string TenantCode { get; }
        public string TenantName { get; }

        public TenantCreatedEvent(int tenantId, string tenantCode, string tenantName)
        {
            TenantId = tenantId;
            TenantCode = tenantCode;
            TenantName = tenantName;
        }
    }
}
