using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Domain.Entities.TenantDb.Common;
using StoockerMT.Domain.Enums;
using StoockerMT.Domain.ValueObjects;

namespace StoockerMT.Domain.Entities.TenantDb
{
    public class Order : TenantBaseEntity
    {
        public int CustomerId { get; private set; }
        public OrderNumber OrderNumber { get; private set; }
        public DateTime OrderDate { get; private set; }
        public DateTime? RequiredDate { get; private set; }
        public DateTime? ShippedDate { get; private set; }
        public Money SubTotal { get; private set; }
        public Money TaxAmount { get; private set; }
        public Money DiscountAmount { get; private set; }
        public Money ShippingAmount { get; private set; }
        public Money Total { get; private set; }
        public OrderStatus Status { get; private set; } = OrderStatus.Pending;
        public string? Notes { get; private set; }
        public Address? ShippingAddress { get; private set; }

        // Navigation Properties
        public virtual Customer Customer { get; set; }
        public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

        private Order() { }

        public Order(int customerId, OrderNumber orderNumber, string currency = "USD", DateTime? requiredDate = null)
        {
            CustomerId = customerId;
            OrderNumber = orderNumber ?? throw new ArgumentNullException(nameof(orderNumber));
            OrderDate = DateTime.UtcNow;
            RequiredDate = requiredDate;

            // Initialize money values
            SubTotal = Money.Zero(currency);
            TaxAmount = Money.Zero(currency);
            DiscountAmount = Money.Zero(currency);
            ShippingAmount = Money.Zero(currency);
            Total = Money.Zero(currency);
        }

        public void SetShippingAddress(Address address)
        {
            ShippingAddress = address;
            UpdateTimestamp();
        }

        public void AddItem(Product product, int quantity, Money unitPrice)
        {
            if (unitPrice.Currency != SubTotal.Currency)
                throw new ArgumentException("Currency mismatch");

            var item = new OrderItem(Id, product.ProductName, quantity, unitPrice, product.Id);
            Items.Add(item);
            CalculateTotals();
        }

        public void ApplyDiscount(Money discountAmount)
        {
            if (discountAmount.Currency != SubTotal.Currency)
                throw new ArgumentException("Currency mismatch");

            if (discountAmount.Amount > SubTotal.Amount)
                throw new InvalidOperationException("Discount cannot be greater than subtotal");

            DiscountAmount = discountAmount;
            CalculateTotals();
        }

        public void ApplyDiscountPercentage(Percentage discountPercentage)
        {
            DiscountAmount = discountPercentage.ApplyTo(SubTotal);
            CalculateTotals();
        }

        public void SetShippingCost(Money shippingAmount)
        {
            if (shippingAmount.Currency != SubTotal.Currency)
                throw new ArgumentException("Currency mismatch");

            ShippingAmount = shippingAmount;
            CalculateTotals();
        }

        public void CalculateTotals()
        {
            if (!Items.Any())
            {
                SubTotal = Money.Zero(SubTotal.Currency);
                Total = Money.Zero(SubTotal.Currency);
                return;
            }

            SubTotal = Items.Aggregate(
                Money.Zero(SubTotal.Currency),
                (sum, item) => sum.Add(item.Total)
            );

            // Calculate tax (assuming 18% tax rate)
            TaxAmount = SubTotal.Multiply(0.18m);

            // Calculate total
            Total = SubTotal
                .Add(TaxAmount)
                .Subtract(DiscountAmount)
                .Add(ShippingAmount);

            UpdateTimestamp();
        }

        public void UpdateStatus(OrderStatus status)
        {
            Status = status;

            if (status == OrderStatus.Shipped)
            {
                ShippedDate = DateTime.UtcNow;
            }

            UpdateTimestamp();
        }

        public void Cancel(string reason)
        {
            if (Status == OrderStatus.Shipped || Status == OrderStatus.Delivered)
                throw new InvalidOperationException("Cannot cancel shipped or delivered orders");

            Status = OrderStatus.Cancelled;
            Notes = $"Cancelled: {reason}";
            UpdateTimestamp();
        }
    }
}
