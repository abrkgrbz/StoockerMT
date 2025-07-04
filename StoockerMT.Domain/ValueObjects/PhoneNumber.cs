using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StoockerMT.Domain.ValueObjects
{
    public class PhoneNumber : BaseValueObject
    {
        private static readonly Regex PhoneRegex = new Regex(
            @"^\+?[1-9]\d{1,14}$", // E.164 format
            RegexOptions.Compiled);

        public string Value { get; private set; }

        private PhoneNumber() { }

        public PhoneNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Phone number cannot be empty", nameof(value));

            // Remove common formatting characters
            var cleaned = value.Replace(" ", "")
                .Replace("-", "")
                .Replace("(", "")
                .Replace(")", "");

            if (!PhoneRegex.IsMatch(cleaned))
                throw new ArgumentException($"Invalid phone number format: {value}", nameof(value));

            Value = cleaned;
        }

        public string GetFormatted()
        {
            // Simple formatting for display
            if (Value.Length == 10) // US format
                return $"({Value.Substring(0, 3)}) {Value.Substring(3, 3)}-{Value.Substring(6)}";

            return Value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => GetFormatted();
    }
}
