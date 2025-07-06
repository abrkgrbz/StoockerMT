using StoockerMT.Domain.Entities.MasterDb;
using StoockerMT.Domain.Repositories.MasterDb;
using StoockerMT.Persistence.Contexts;
using StoockerMT.Persistence.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace StoockerMT.Persistence.Repositories.MasterDb
{
    public class TenantModuleUsageRepository : RepositoryBase<TenantModuleUsage>, ITenantModuleUsageRepository
    {
        private readonly MasterDbContext _context;

        public TenantModuleUsageRepository(MasterDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<TenantModuleUsage>> GetBySubscriptionAsync(int subscriptionId, CancellationToken cancellationToken = default)
        {
            return await _context.TenantModuleUsages
                .Where(u => u.SubscriptionId == subscriptionId)
                .OrderByDescending(u => u.UsageDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<TenantModuleUsage>> GetByDateRangeAsync(int subscriptionId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await _context.TenantModuleUsages
                .Where(u =>
                    u.SubscriptionId == subscriptionId &&
                    u.UsageDate >= startDate &&
                    u.UsageDate <= endDate)
                .OrderByDescending(u => u.UsageDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetTotalUsageAsync(int subscriptionId, string feature, CancellationToken cancellationToken = default)
        {
            return await _context.TenantModuleUsages
                .Where(u =>
                    u.SubscriptionId == subscriptionId &&
                    u.Feature == feature)
                .SumAsync(u => u.UsageCount, cancellationToken);
        }
    }
}
