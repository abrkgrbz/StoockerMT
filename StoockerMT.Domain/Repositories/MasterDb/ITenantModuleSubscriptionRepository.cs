using StoockerMT.Domain.Entities.MasterDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Repositories.MasterDb
{
    public interface ITenantModuleSubscriptionRepository : IRepository<TenantModuleSubscription>
    {
        Task<TenantModuleSubscription?> GetActiveSubscriptionAsync(int tenantId, int moduleId, CancellationToken cancellationToken = default);
        Task<TenantModuleSubscription?> GetWithUsageAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TenantModuleSubscription>> GetByTenantAsync(int tenantId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TenantModuleSubscription>> GetByModuleAsync(int moduleId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TenantModuleSubscription>> GetExpiringSubscriptionsAsync(int daysBeforeExpiry, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TenantModuleSubscription>> GetExpiredSubscriptionsAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TenantModuleSubscription>> GetAutoRenewSubscriptionsAsync(CancellationToken cancellationToken = default);
        Task<bool> HasActiveSubscriptionAsync(int tenantId, int moduleId, CancellationToken cancellationToken = default);
    }
}
