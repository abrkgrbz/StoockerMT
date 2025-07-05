using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Exceptions.Business
{
    public class InsufficientStockException : DomainException
    {
        public int ProductId { get; }
        public decimal RequestedQuantity { get; }
        public decimal AvailableQuantity { get; }

        public InsufficientStockException(int productId, decimal requestedQuantity, decimal availableQuantity)
            : base("INSUFFICIENT_STOCK",
                $"Insufficient stock for product '{productId}'. Requested: {requestedQuantity}, Available: {availableQuantity}")
        {
            ProductId = productId;
            RequestedQuantity = requestedQuantity;
            AvailableQuantity = availableQuantity;
        }
    }
}
