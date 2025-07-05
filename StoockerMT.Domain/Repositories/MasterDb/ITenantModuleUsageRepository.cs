using StoockerMT.Domain.Entities.MasterDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Repositories.MasterDb
{
    public interface ITenantModuleUsageRepository : IRepository<TenantModuleUsage>
    {
        Task<IReadOnlyList<TenantModuleUsage>> GetBySubscriptionAsync(int subscriptionId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TenantModuleUsage>> GetByDateRangeAsync(int subscriptionId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<int> GetTotalUsageAsync(int subscriptionId, string feature, CancellationToken cancellationToken = default);
    }
}
