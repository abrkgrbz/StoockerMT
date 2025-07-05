using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Exceptions.Tenant
{
    public class TenantAlreadyExistsException : DomainException
    {
        public string TenantCode { get; }

        public TenantAlreadyExistsException(string tenantCode)
            : base("TENANT_ALREADY_EXISTS", $"Tenant with code '{tenantCode}' already exists.")
        {
            TenantCode = tenantCode;
        }
    }
}
