using StoockerMT.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Events.Order
{
    public class OrderCancelledEvent : DomainEvent
    {
        public int OrderId { get; }
        public string Reason { get; }
        public Money RefundAmount { get; }

        public OrderCancelledEvent(int orderId, string reason, Money refundAmount)
        {
            OrderId = orderId;
            Reason = reason;
            RefundAmount = refundAmount;
        }
    }
}
