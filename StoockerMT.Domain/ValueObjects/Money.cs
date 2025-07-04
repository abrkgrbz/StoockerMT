using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.ValueObjects
{
    public class Money : BaseValueObject
    {
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }

        private Money() { }

        public Money(decimal amount, string currency = "TRY")
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative", nameof(amount));

            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency cannot be empty", nameof(currency));

            Amount = Math.Round(amount, 2);
            Currency = currency.ToUpperInvariant();
        }

        public Money Add(Money other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (Currency != other.Currency)
                throw new InvalidOperationException($"Cannot add money with different currencies: {Currency} and {other.Currency}");

            return new Money(Amount + other.Amount, Currency);
        }

        public Money Subtract(Money other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (Currency != other.Currency)
                throw new InvalidOperationException($"Cannot subtract money with different currencies: {Currency} and {other.Currency}");

            return new Money(Amount - other.Amount, Currency);
        }

        public Money Multiply(decimal factor)
        {
            return new Money(Amount * factor, Currency);
        }

        public Money ApplyDiscount(Percentage discount)
        {
            var discountAmount = Amount * discount.Value / 100;
            return new Money(Amount - discountAmount, Currency);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }

        public override string ToString()
        {
            return $"{Currency} {Amount:N2}";
        }

        public static Money Zero(string currency = "TRY") => new Money(0, currency);
    }
}
