using StoockerMT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Exceptions.Tenant
{
    public class TenantInactiveException : DomainException
    {
        public int TenantId { get; }
        public TenantStatus Status { get; }

        public TenantInactiveException(int tenantId, TenantStatus status)
            : base("TENANT_INACTIVE", $"Tenant '{tenantId}' is not active. Current status: {status}")
        {
            TenantId = tenantId;
            Status = status;
        }
    }
}
