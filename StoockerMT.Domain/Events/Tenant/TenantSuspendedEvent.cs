using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Events.Tenant
{
    public class TenantSuspendedEvent : DomainEvent
    {
        public int TenantId { get; }
        public string Reason { get; }
        public string SuspendedBy { get; }

        public TenantSuspendedEvent(int tenantId, string reason, string suspendedBy)
        {
            TenantId = tenantId;
            Reason = reason;
            SuspendedBy = suspendedBy;
        }
    }
}
