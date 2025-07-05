using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Events
{
    public abstract class DomainEvent
    {
        public DateTime OccurredAt { get; }
        public Guid EventId { get; }

        protected DomainEvent()
        {
            OccurredAt = DateTime.UtcNow;
            EventId = Guid.NewGuid();
        }
    }
}
