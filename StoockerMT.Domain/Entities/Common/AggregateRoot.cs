using StoockerMT.Domain.Entities.TenantDb.Common;
using StoockerMT.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Entities.Common
{
    public abstract class AggregateRoot : BaseEntity
    {
        private readonly List<DomainEvent> _domainEvents = new List<DomainEvent>();
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }

    // Tenant DB için AggregateRoot
    public abstract class TenantAggregateRoot : TenantBaseEntity
    {
        private readonly List<DomainEvent> _domainEvents = new List<DomainEvent>();
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
