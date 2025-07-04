using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StoockerMT.Domain.ValueObjects
{
    public class AccountCode : BaseValueObject
    {
        private static readonly Regex CodeRegex = new Regex(
            @"^\d{4,8}$", // 4-8 digit numeric code
            RegexOptions.Compiled);

        public string Value { get; private set; }

        private AccountCode() { }

        public AccountCode(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Account code cannot be empty", nameof(value));

            if (!CodeRegex.IsMatch(value))
                throw new ArgumentException(
                    "Account code must be 4-8 digits",
                    nameof(value));

            Value = value;
        }

        public bool IsSubAccountOf(AccountCode parent)
        {
            if (parent == null) return false;

            return Value.StartsWith(parent.Value) && Value.Length > parent.Value.Length;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
