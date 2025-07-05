using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Events.Tenant
{
    public class TenantActivatedEvent : DomainEvent
    {
        public int TenantId { get; }
        public string ActivatedBy { get; }

        public TenantActivatedEvent(int tenantId, string activatedBy)
        {
            TenantId = tenantId;
            ActivatedBy = activatedBy;
        }
    }
}
