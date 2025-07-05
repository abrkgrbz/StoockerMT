using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Events.Order
{
    public class OrderShippedEvent : DomainEvent
    {
        public int OrderId { get; }
        public DateTime ShippedDate { get; }
        public string TrackingNumber { get; }

        public OrderShippedEvent(int orderId, DateTime shippedDate, string trackingNumber)
        {
            OrderId = orderId;
            ShippedDate = shippedDate;
            TrackingNumber = trackingNumber;
        }
    }
}
