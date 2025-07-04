using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.ValueObjects
{
    public class Quantity : BaseValueObject
    {
        public decimal Value { get; private set; }
        public string Unit { get; private set; }

        private Quantity() { }

        public Quantity(decimal value, string unit = "PCS")
        {
            if (value < 0)
                throw new ArgumentException("Quantity cannot be negative", nameof(value));

            if (string.IsNullOrWhiteSpace(unit))
                throw new ArgumentException("Unit cannot be empty", nameof(unit));

            Value = value;
            Unit = unit.ToUpperInvariant();
        }

        public Quantity Add(Quantity other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (Unit != other.Unit)
                throw new InvalidOperationException($"Cannot add quantities with different units: {Unit} and {other.Unit}");

            return new Quantity(Value + other.Value, Unit);
        }

        public Quantity Subtract(Quantity other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (Unit != other.Unit)
                throw new InvalidOperationException($"Cannot subtract quantities with different units: {Unit} and {other.Unit}");

            return new Quantity(Value - other.Value, Unit);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
            yield return Unit;
        }

        public override string ToString() => $"{Value} {Unit}";

        public static Quantity Zero(string unit = "PCS") => new Quantity(0, unit);
    }
}
