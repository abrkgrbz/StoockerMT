using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Exceptions.Tenant
{
    public class TenantNotFoundException : DomainException
    {
        public int? TenantId { get; }
        public string TenantCode { get; }

        public TenantNotFoundException(int tenantId)
            : base("TENANT_NOT_FOUND", $"Tenant with ID '{tenantId}' was not found.")
        {
            TenantId = tenantId;
        }

        public TenantNotFoundException(string tenantCode)
            : base("TENANT_NOT_FOUND", $"Tenant with code '{tenantCode}' was not found.")
        {
            TenantCode = tenantCode;
        }
    }
}
