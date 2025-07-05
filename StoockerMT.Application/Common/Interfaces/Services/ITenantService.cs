using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Interfaces.Services
{
    public interface ITenantService
    {
        Task<string> CreateTenantDatabaseAsync(int tenantId, string tenantCode, CancellationToken cancellationToken = default);
        Task<bool> MigrateTenantDatabaseAsync(int tenantId, CancellationToken cancellationToken = default);
        Task<bool> DeleteTenantDatabaseAsync(int tenantId, CancellationToken cancellationToken = default);
        Task<bool> BackupTenantDatabaseAsync(int tenantId, CancellationToken cancellationToken = default);
        Task<bool> RestoreTenantDatabaseAsync(int tenantId, string backupPath, CancellationToken cancellationToken = default);
        Task<bool> CheckTenantDatabaseHealthAsync(int tenantId, CancellationToken cancellationToken = default);
    }
}
