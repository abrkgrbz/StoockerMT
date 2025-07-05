using StoockerMT.Domain.Enums;
using StoockerMT.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Events.Inventory
{
    public class InventoryMovementCreatedEvent : DomainEvent
    {
        public int ProductId { get; }
        public MovementType Type { get; }
        public Quantity Quantity { get; }
        public Money TotalValue { get; }

        public InventoryMovementCreatedEvent(int productId, MovementType type, Quantity quantity, Money totalValue)
        {
            ProductId = productId;
            Type = type;
            Quantity = quantity;
            TotalValue = totalValue;
        }
    }
}
