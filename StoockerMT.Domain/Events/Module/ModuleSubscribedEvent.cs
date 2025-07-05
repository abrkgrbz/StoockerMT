using StoockerMT.Domain.Enums;
using StoockerMT.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Events.Module
{
    public class ModuleSubscribedEvent : DomainEvent
    {
        public int TenantId { get; }
        public int ModuleId { get; }
        public string ModuleCode { get; }
        public SubscriptionType SubscriptionType { get; }
        public Money Amount { get; }

        public ModuleSubscribedEvent(int tenantId, int moduleId, string moduleCode,
            SubscriptionType subscriptionType, Money amount)
        {
            TenantId = tenantId;
            ModuleId = moduleId;
            ModuleCode = moduleCode;
            SubscriptionType = subscriptionType;
            Amount = amount;
        }
    }
}
