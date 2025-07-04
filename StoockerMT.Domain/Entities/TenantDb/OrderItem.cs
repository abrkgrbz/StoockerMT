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
    public class OrderItem : TenantBaseEntity
    {
        public int OrderId { get; private set; }
        public int? ProductId { get; private set; }
        public string ProductName { get; private set; }
        public string? ProductCode { get; private set; }
        public Quantity Quantity { get; private set; }
        public Money UnitPrice { get; private set; }
        public Money DiscountAmount { get; private set; }
        public Money Total { get; private set; }

        // Navigation Properties
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }

        private OrderItem() { }

        public OrderItem(int orderId, string productName, int quantity, Money unitPrice, int? productId = null)
        {
            OrderId = orderId;
            ProductName = productName ?? throw new ArgumentNullException(nameof(productName));
            Quantity = new Quantity(quantity, "PCS");
            UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
            DiscountAmount = Money.Zero(unitPrice.Currency);
            ProductId = productId;
            CalculateTotal();
        }

        public void ApplyDiscount(Money discountAmount)
        {
            if (discountAmount.Currency != UnitPrice.Currency)
                throw new ArgumentException("Currency mismatch");

            var maxDiscount = UnitPrice.Multiply(Quantity.Value);
            if (discountAmount.Amount > maxDiscount.Amount)
                throw new InvalidOperationException("Discount cannot be greater than item total");

            DiscountAmount = discountAmount;
            CalculateTotal();
        }

        public void ApplyDiscountPercentage(Percentage discountPercentage)
        {
            var itemTotal = UnitPrice.Multiply(Quantity.Value);
            DiscountAmount = discountPercentage.ApplyTo(itemTotal);
            CalculateTotal();
        }

        private void CalculateTotal()
        {
            var subtotal = UnitPrice.Multiply(Quantity.Value);
            Total = subtotal.Subtract(DiscountAmount);
            UpdateTimestamp();
        }
    }
}
