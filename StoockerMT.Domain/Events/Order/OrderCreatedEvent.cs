using StoockerMT.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Events.Order
{
    public class OrderCreatedEvent : DomainEvent
    {
        public int OrderId { get; }
        public string OrderNumber { get; }
        public int CustomerId { get; }
        public Money Total { get; }

        public OrderCreatedEvent(int orderId, string orderNumber, int customerId, Money total)
        {
            OrderId = orderId;
            OrderNumber = orderNumber;
            CustomerId = customerId;
            Total = total;
        }
    }
}
