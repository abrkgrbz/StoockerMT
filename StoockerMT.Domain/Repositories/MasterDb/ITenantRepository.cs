using StoockerMT.Domain.Entities.MasterDb;
using StoockerMT.Domain.Enums;
using StoockerMT.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Repositories.MasterDb
{
    public interface ITenantRepository : IRepository<Tenant>
    {
        Task<Tenant?> GetByCodeAsync(TenantCode code, CancellationToken cancellationToken = default);
        Task<Tenant?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<Tenant?> GetWithModulesAsync(int id, CancellationToken cancellationToken = default);
        Task<Tenant?> GetWithUsersAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Tenant>> GetByStatusAsync(TenantStatus status, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Tenant>> GetActiveTenantsAsync(CancellationToken cancellationToken = default);
        Task<bool> ExistsByCodeAsync(TenantCode code, CancellationToken cancellationToken = default);
        Task<bool> IsConnectionStringUniqueAsync(string connectionString, int? excludeTenantId = null, CancellationToken cancellationToken = default);
    }
}
