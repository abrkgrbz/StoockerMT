using Microsoft.AspNetCore.Http;
using StoockerMT.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Persistence.Services
{
    public class CurrentTenantService : ICurrentTenantService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentTenantService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? TenantId
        {
            get
            {
                var tenantIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("TenantId");
                if (tenantIdClaim != null && int.TryParse(tenantIdClaim.Value, out var tenantId))
                {
                    return tenantId;
                }

                // Try header
                var tenantHeader = _httpContextAccessor.HttpContext?.Request.Headers["X-Tenant-Id"].FirstOrDefault();
                if (!string.IsNullOrEmpty(tenantHeader) && int.TryParse(tenantHeader, out tenantId))
                {
                    return tenantId;
                }

                return null;
            }
        }

        public string? TenantCode
        {
            get
            {
                var tenantCodeClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("TenantCode");
                if (tenantCodeClaim != null)
                {
                    return tenantCodeClaim.Value;
                }

                // Try header
                return _httpContextAccessor.HttpContext?.Request.Headers["X-Tenant-Code"].FirstOrDefault();
            }
        }

        public string? ConnectionString { get; set; }

        public bool HasTenant => TenantId.HasValue;
    }
}
