using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StoockerMT.Domain.ValueObjects
{
    public class InvoiceNumber : BaseValueObject
    {
        private static readonly Regex InvoiceNumberRegex = new Regex(
            @"^INV-\d{4}-\d{6}$", // Format: INV-YYYY-000001
            RegexOptions.Compiled);

        public string Value { get; private set; }

        private InvoiceNumber() { }

        public InvoiceNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Invoice number cannot be empty", nameof(value));

            if (!InvoiceNumberRegex.IsMatch(value))
                throw new ArgumentException($"Invalid invoice number format: {value}", nameof(value));

            Value = value;
        }

        public static InvoiceNumber Generate(string tenantCode)
        {
            var year = DateTime.UtcNow.Year;
            var sequence = new Random().Next(1, 999999); // In production, use a proper sequence
            return new InvoiceNumber($"INV-{year}-{sequence:D6}");
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
