using StoockerMT.Domain.Entities.MasterDb;
using StoockerMT.Domain.Enums;
using StoockerMT.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Services
{
    public interface ISubscriptionService
    {
        bool CanAccessModule(Tenant tenant, Module module);
        Money CalculateSubscriptionCost(Module module, SubscriptionType type, Percentage discount = null);
        DateRange CalculateSubscriptionPeriod(DateTime startDate, SubscriptionType type);
        bool ShouldRenewSubscription(TenantModuleSubscription subscription);
    }

    public class SubscriptionService : ISubscriptionService
    {
        public bool CanAccessModule(Tenant tenant, Module module)
        {
            if (tenant.Status != TenantStatus.Active)
                return false;

            var subscription = tenant.ModuleSubscriptions
                .FirstOrDefault(s => s.ModuleId == module.Id);

            return subscription != null && subscription.IsActive;
        }

        public Money CalculateSubscriptionCost(Module module, SubscriptionType type, Percentage discount = null)
        {
            var basePrice = type == SubscriptionType.Monthly
                ? module.MonthlyPrice
                : module.YearlyPrice;

            return discount != null
                ? basePrice.ApplyDiscount(discount)
                : basePrice;
        }

        public DateRange CalculateSubscriptionPeriod(DateTime startDate, SubscriptionType type)
        {
            var endDate = type == SubscriptionType.Monthly
                ? startDate.AddMonths(1).AddDays(-1)
                : startDate.AddYears(1).AddDays(-1);

            return new DateRange(startDate, endDate);
        }

        public bool ShouldRenewSubscription(TenantModuleSubscription subscription)
        {
            return subscription.AutoRenew &&
                   subscription.Status == SubscriptionStatus.Active &&
                   subscription.SubscriptionPeriod.EndDate <= DateTime.UtcNow.AddDays(7); // 7 days before expiry
        }
    }
}
