using System;
using StoockerMT.Application.Common.Specifications;
using StoockerMT.Domain.Entities.MasterDb;
using StoockerMT.Domain.Enums;
using StoockerMT.Domain.ValueObjects;

namespace StoockerMT.Application.Specifications.MasterDb
{
    // Active tenants specification
    public class ActiveTenantsSpecification : BaseSpecification<Tenant>
    {
        public ActiveTenantsSpecification()
            : base(t => t.Status == TenantStatus.Active && !t.IsDeleted)
        {
            ApplyOrderBy(t => t.Name);
        }
    }

    // Tenant by code specification
    public class TenantByCodeSpecification : BaseSpecification<Tenant>
    {
        public TenantByCodeSpecification(string code)
            : base(t => t.Code.Value == code && !t.IsDeleted)
        {
            AddInclude(t => t.ModuleSubscriptions);
            AddInclude("ModuleSubscriptions.Module");
            ApplyNoTracking();
        }
    }

    // Tenants with expiring subscriptions
    public class TenantsWithExpiringSubscriptionsSpecification : BaseSpecification<Tenant>
    {
        public TenantsWithExpiringSubscriptionsSpecification(int daysBeforeExpiry)
        {
            var expiryDate = DateTime.UtcNow.AddDays(daysBeforeExpiry);

            Criteria = t => t.Status == TenantStatus.Active &&
                            t.ModuleSubscriptions.Any(s =>
                                s.Status == SubscriptionStatus.Active &&
                                s.SubscriptionPeriod.EndDate <= expiryDate);

            AddInclude(t => t.ModuleSubscriptions);
            AddInclude("ModuleSubscriptions.Module");
        }
    }
}