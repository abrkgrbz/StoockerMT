using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Events.Module
{
    public class ModuleUnsubscribedEvent : DomainEvent
    {
        public int TenantId { get; }
        public int ModuleId { get; }
        public string Reason { get; }

        public ModuleUnsubscribedEvent(int tenantId, int moduleId, string reason)
        {
            TenantId = tenantId;
            ModuleId = moduleId;
            Reason = reason;
        }
    }
}
