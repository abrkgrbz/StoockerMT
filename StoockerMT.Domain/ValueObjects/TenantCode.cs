using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StoockerMT.Domain.ValueObjects
{
    public class TenantCode : BaseValueObject
    {
        private static readonly Regex CodeRegex = new Regex(
            @"^[A-Z][A-Z0-9]{2,9}$", // Start with letter, 3-10 chars total
            RegexOptions.Compiled);

        public string Value { get; private set; }

        private TenantCode() { }

        public TenantCode(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Tenant code cannot be empty", nameof(value));

            var upperValue = value.ToUpperInvariant();

            if (!CodeRegex.IsMatch(upperValue))
                throw new ArgumentException(
                    "Tenant code must start with a letter and be 3-10 alphanumeric characters",
                    nameof(value));

            Value = upperValue;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;

        public static implicit operator string(TenantCode code) => code?.Value;
    }
}
