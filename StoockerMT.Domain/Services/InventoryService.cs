using StoockerMT.Domain.Entities.TenantDb;
using StoockerMT.Domain.Exceptions.Business;
using StoockerMT.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Services
{
    public interface IInventoryService
    {
        bool CanFulfillOrder(Product product, Quantity requestedQuantity);
        void ReserveStock(Product product, Quantity quantity);
        void ReleaseStock(Product product, Quantity quantity);
        IEnumerable<Product> GetLowStockProducts(IEnumerable<Product> products);
    }

    public class InventoryService : IInventoryService
    {
        public bool CanFulfillOrder(Product product, Quantity requestedQuantity)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));
            if (requestedQuantity == null) throw new ArgumentNullException(nameof(requestedQuantity));

            return product.StockQuantity.Value >= requestedQuantity.Value &&
                   product.StockQuantity.Unit == requestedQuantity.Unit;
        }

        public void ReserveStock(Product product, Quantity quantity)
        {
            if (!CanFulfillOrder(product, quantity))
                throw new InsufficientStockException(product.Id, quantity.Value, product.StockQuantity.Value);

            product.AdjustStock(-((int)quantity.Value));
        }

        public void ReleaseStock(Product product, Quantity quantity)
        {
            product.AdjustStock((int)quantity.Value);
        }

        public IEnumerable<Product> GetLowStockProducts(IEnumerable<Product> products)
        {
            return products.Where(p => p.IsLowStock());
        }
    }
}
