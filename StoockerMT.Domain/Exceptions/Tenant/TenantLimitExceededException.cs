using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Exceptions.Tenant
{
    public class TenantLimitExceededException : DomainException
    {
        public string LimitType { get; }
        public int CurrentValue { get; }
        public int MaxValue { get; }

        public TenantLimitExceededException(string limitType, int currentValue, int maxValue)
            : base("TENANT_LIMIT_EXCEEDED",
                $"Tenant {limitType} limit exceeded. Current: {currentValue}, Max: {maxValue}")
        {
            LimitType = limitType;
            CurrentValue = currentValue;
            MaxValue = maxValue;
        }
    }
}
