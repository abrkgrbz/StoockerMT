using StoockerMT.Domain.Entities.MasterDb;
using StoockerMT.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Repositories.MasterDb
{
    public interface ITenantUserRepository : IRepository<TenantUser>
    {
        Task<TenantUser?> GetByEmailAsync(Email email, int tenantId, CancellationToken cancellationToken = default);
        Task<TenantUser?> GetWithPermissionsAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TenantUser>> GetByTenantAsync(int tenantId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TenantUser>> GetAdminsByTenantAsync(int tenantId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TenantUser>> GetActiveUsersByTenantAsync(int tenantId, CancellationToken cancellationToken = default);
        Task<bool> ExistsByEmailAsync(Email email, int tenantId, CancellationToken cancellationToken = default);
        Task<int> GetUserCountByTenantAsync(int tenantId, CancellationToken cancellationToken = default);
    }
}
