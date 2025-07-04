using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.ValueObjects
{
    public class TenantId : IEquatable<TenantId>
    {
        public int Value { get; }

        public TenantId(int value)
        {
            if (value <= 0)
                throw new ArgumentException("Tenant ID must be greater than zero", nameof(value));

            Value = value;
        }

        public bool Equals(TenantId other)
        {
            return other != null && Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is TenantId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static implicit operator int(TenantId tenantId)
        {
            return tenantId.Value;
        }

        public static implicit operator TenantId(int value)
        {
            return new TenantId(value);
        }
    }
}
