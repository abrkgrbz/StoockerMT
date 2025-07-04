using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.ValueObjects
{
    public class Percentage : BaseValueObject
    {
        public decimal Value { get; private set; }

        private Percentage() { }

        public Percentage(decimal value)
        {
            if (value < 0)
                throw new ArgumentException("Percentage cannot be negative", nameof(value));

            if (value > 100)
                throw new ArgumentException("Percentage cannot be greater than 100", nameof(value));

            Value = Math.Round(value, 2);
        }

        public decimal ToDecimal() => Value / 100m;

        public Money ApplyTo(Money money)
        {
            return money.Multiply(ToDecimal());
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => $"{Value}%";

        public static Percentage Zero => new Percentage(0);
        public static Percentage Hundred => new Percentage(100);
    }
}
