using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Interfaces
{
    public interface ICurrentTenantService
    { 
        int? TenantId { get; }
        string TenantCode { get; }
        string TenantName { get; }
        string ConnectionString { get; }
         
        int? UserId { get; }
        string UserEmail { get; }
        bool IsTenantAdmin { get; }

        // Tenant Resolution
        Task<bool> SetTenantAsync(string tenantIdentifier);
        Task<bool> SetTenantByIdAsync(int tenantId);
        Task<bool> SetTenantByCodeAsync(string tenantCode);
        Task<bool> SetTenantByDomainAsync(string domain);

        // Validation
        bool HasTenant();
        bool IsAuthenticated();
        Task<bool> ValidateTenantAccessAsync();
    }
}
