using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Domain.Entities.TenantDb.Common;
using StoockerMT.Domain.ValueObjects;

namespace StoockerMT.Domain.Entities.TenantDb
{
    public class Product : TenantBaseEntity
    {
        public string ProductName { get; private set; }
        public string? ProductCode { get; private set; }
        public string? SKU { get; private set; }
        public string? Description { get; private set; }
        public int? CategoryId { get; private set; }
        public Money UnitPrice { get; private set; }
        public Money CostPrice { get; private set; }
        public Quantity StockQuantity { get; private set; }
        public Quantity MinimumStockLevel { get; private set; }
        public bool IsActive { get; private set; } = true;
        public decimal Weight { get; private set; }
        public string? ImageUrl { get; private set; }

        // Navigation Properties
        public virtual ProductCategory Category { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();

        private Product() { }

        public Product(string productName, Money unitPrice, Money costPrice, string unit = "PCS")
        {
            ProductName = productName ?? throw new ArgumentNullException(nameof(productName));
            UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
            CostPrice = costPrice ?? throw new ArgumentNullException(nameof(costPrice));

            if (unitPrice.Currency != costPrice.Currency)
                throw new ArgumentException("Unit price and cost price must have the same currency");

            StockQuantity = Quantity.Zero(unit);
            MinimumStockLevel = new Quantity(10, unit); // Default minimum
        }

        public void UpdateStock(Quantity quantity)
        {
            if (quantity.Unit != StockQuantity.Unit)
                throw new ArgumentException("Unit mismatch");

            StockQuantity = quantity;
            UpdateTimestamp();
        }

        public void AdjustStock(int quantityChange)
        {
            var newQuantity = StockQuantity.Value + quantityChange;
            if (newQuantity < 0)
                throw new InvalidOperationException("Stock cannot be negative");

            StockQuantity = new Quantity(newQuantity, StockQuantity.Unit);
            UpdateTimestamp();
        }

        public void UpdatePricing(Money unitPrice, Money costPrice)
        {
            if (unitPrice.Currency != costPrice.Currency)
                throw new ArgumentException("Unit price and cost price must have the same currency");

            UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
            CostPrice = costPrice ?? throw new ArgumentNullException(nameof(costPrice));
            UpdateTimestamp();
        }

        public void SetMinimumStockLevel(Quantity minimumLevel)
        {
            if (minimumLevel.Unit != StockQuantity.Unit)
                throw new ArgumentException("Unit mismatch");

            MinimumStockLevel = minimumLevel;
            UpdateTimestamp();
        }

        public bool IsLowStock()
        {
            return StockQuantity.Value <= MinimumStockLevel.Value;
        }

        public Money GetProfitMargin()
        {
            return UnitPrice.Subtract(CostPrice);
        }

        public Percentage GetProfitMarginPercentage()
        {
            if (CostPrice.Amount == 0)
                return new Percentage(100);

            var margin = (UnitPrice.Amount - CostPrice.Amount) / CostPrice.Amount * 100;
            return new Percentage(margin);
        }
    }
} 
