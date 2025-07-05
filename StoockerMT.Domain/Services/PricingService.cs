using StoockerMT.Domain.Entities.TenantDb;
using StoockerMT.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Services
{
    public interface IPricingService
    {
        Money CalculateOrderTotal(IEnumerable<OrderItem> items, Money shippingCost, Percentage discountPercentage);
        Money CalculateTax(Money amount, string country);
        Money ApplyVolumeDiscount(Money basePrice, int quantity);
    }

    public class PricingService : IPricingService
    {
        private readonly Dictionary<string, decimal> _taxRates = new()
        {
            { "TR", 0.18m },
            { "US", 0.08m },
            { "UK", 0.20m },
            { "DE", 0.19m }
        };

        public Money CalculateOrderTotal(IEnumerable<OrderItem> items, Money shippingCost, Percentage discountPercentage)
        {
            var subtotal = items.Aggregate(
                Money.Zero(items.First().UnitPrice.Currency),
                (sum, item) => sum.Add(item.Total)
            );

            var discountAmount = discountPercentage.ApplyTo(subtotal);
            var afterDiscount = subtotal.Subtract(discountAmount);
            var tax = CalculateTax(afterDiscount, "TR"); // Default to TR

            return afterDiscount.Add(tax).Add(shippingCost);
        }

        public Money CalculateTax(Money amount, string country)
        {
            if (!_taxRates.TryGetValue(country, out var taxRate))
                taxRate = 0.18m; // Default tax rate

            return amount.Multiply(taxRate);
        }

        public Money ApplyVolumeDiscount(Money basePrice, int quantity)
        {
            var discountPercentage = quantity switch
            {
                >= 100 => new Percentage(15),
                >= 50 => new Percentage(10),
                >= 20 => new Percentage(5),
                _ => Percentage.Zero
            };

            return basePrice.ApplyDiscount(discountPercentage);
        }
    }
}
