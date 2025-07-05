using StoockerMT.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Events.Inventory
{
    public class StockLevelLowEvent : DomainEvent
    {
        public int ProductId { get; }
        public string ProductName { get; }
        public Quantity CurrentStock { get; }
        public Quantity MinimumLevel { get; }

        public StockLevelLowEvent(int productId, string productName, Quantity currentStock, Quantity minimumLevel)
        {
            ProductId = productId;
            ProductName = productName;
            CurrentStock = currentStock;
            MinimumLevel = minimumLevel;
        }
    }
}
