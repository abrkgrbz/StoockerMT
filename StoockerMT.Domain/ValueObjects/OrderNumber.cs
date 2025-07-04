using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StoockerMT.Domain.ValueObjects
{
    public class OrderNumber : BaseValueObject
    {
        private static readonly Regex OrderNumberRegex = new Regex(
            @"^ORD-\d{4}-\d{6}$", // Format: ORD-YYYY-000001
            RegexOptions.Compiled);

        public string Value { get; private set; }

        private OrderNumber() { }

        public OrderNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Order number cannot be empty", nameof(value));

            if (!OrderNumberRegex.IsMatch(value))
                throw new ArgumentException($"Invalid order number format: {value}", nameof(value));

            Value = value;
        }

        public static OrderNumber Generate()
        {
            var year = DateTime.UtcNow.Year;
            var sequence = new Random().Next(1, 999999); // In production, use a proper sequence
            return new OrderNumber($"ORD-{year}-{sequence:D6}");
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
