using StoockerMT.Domain.Entities.MasterDb;
using StoockerMT.Domain.Enums;
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
    public class TenantModuleSubscriptionRepository : RepositoryBase<TenantModuleSubscription>, ITenantModuleSubscriptionRepository
    {
        private readonly MasterDbContext _context;

        public TenantModuleSubscriptionRepository(MasterDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<TenantModuleSubscription?> GetActiveSubscriptionAsync(int tenantId, int moduleId, CancellationToken cancellationToken = default)
        {
            return await _context.TenantModuleSubscriptions
                .Include(s => s.Module)
                .FirstOrDefaultAsync(s =>
                    s.TenantId == tenantId &&
                    s.ModuleId == moduleId &&
                    s.Status == SubscriptionStatus.Active &&
                    s.SubscriptionPeriod.StartDate <= DateTime.UtcNow &&
                    s.SubscriptionPeriod.EndDate >= DateTime.UtcNow,
                    cancellationToken);
        }

        public async Task<TenantModuleSubscription?> GetWithUsageAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.TenantModuleSubscriptions
                .Include(s => s.UsageRecords)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<TenantModuleSubscription>> GetByTenantAsync(int tenantId, CancellationToken cancellationToken = default)
        {
            return await _context.TenantModuleSubscriptions
                .Include(s => s.Module)
                .Where(s => s.TenantId == tenantId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<TenantModuleSubscription>> GetByModuleAsync(int moduleId, CancellationToken cancellationToken = default)
        {
            return await _context.TenantModuleSubscriptions
                .Include(s => s.Tenant)
                .Where(s => s.ModuleId == moduleId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<TenantModuleSubscription>> GetExpiringSubscriptionsAsync(int daysBeforeExpiry, CancellationToken cancellationToken = default)
        {
            var expiryDate = DateTime.UtcNow.AddDays(daysBeforeExpiry);

            return await _context.TenantModuleSubscriptions
                .Include(s => s.Tenant)
                .Include(s => s.Module)
                .Where(s =>
                    s.Status == SubscriptionStatus.Active &&
                    s.SubscriptionPeriod.EndDate <= expiryDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<TenantModuleSubscription>> GetExpiredSubscriptionsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.TenantModuleSubscriptions
                .Include(s => s.Tenant)
                .Include(s => s.Module)
                .Where(s =>
                    s.Status == SubscriptionStatus.Active &&
                    s.SubscriptionPeriod.EndDate < DateTime.UtcNow)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<TenantModuleSubscription>> GetAutoRenewSubscriptionsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.TenantModuleSubscriptions
                .Include(s => s.Tenant)
                .Include(s => s.Module)
                .Where(s =>
                    s.AutoRenew &&
                    s.Status == SubscriptionStatus.Active &&
                    s.SubscriptionPeriod.EndDate <= DateTime.UtcNow.AddDays(7))
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> HasActiveSubscriptionAsync(int tenantId, int moduleId, CancellationToken cancellationToken = default)
        {
            return await _context.TenantModuleSubscriptions
                .AnyAsync(s =>
                    s.TenantId == tenantId &&
                    s.ModuleId == moduleId &&
                    s.Status == SubscriptionStatus.Active &&
                    s.SubscriptionPeriod.StartDate <= DateTime.UtcNow &&
                    s.SubscriptionPeriod.EndDate >= DateTime.UtcNow,
                    cancellationToken);
        }
    }
}
