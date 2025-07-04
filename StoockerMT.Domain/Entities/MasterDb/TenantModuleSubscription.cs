using StoockerMT.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Domain.Enums;
using StoockerMT.Domain.ValueObjects;

namespace StoockerMT.Domain.Entities.MasterDb
{
    public class TenantModuleSubscription : BaseEntity, IAuditableEntity
    {
        public int TenantId { get; private set; }
        public int ModuleId { get; private set; }
        public DateRange SubscriptionPeriod { get; private set; }
        public SubscriptionType SubscriptionType { get; private set; }
        public SubscriptionStatus Status { get; private set; } = SubscriptionStatus.Active;
        public Money Amount { get; private set; }
        public Money? DiscountAmount { get; private set; }
        public bool AutoRenew { get; private set; } = true;
        public string CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public virtual Tenant Tenant { get; set; }
        public virtual Module Module { get; set; }
        public virtual ICollection<TenantModuleUsage> UsageRecords { get; set; } = new List<TenantModuleUsage>();

        private TenantModuleSubscription() { }

        public TenantModuleSubscription(int tenantId, int moduleId, SubscriptionType subscriptionType, Money amount, string createdBy)
        {
            TenantId = tenantId;
            ModuleId = moduleId;
            SubscriptionType = subscriptionType;
            Amount = amount ?? throw new ArgumentNullException(nameof(amount));
            CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));
            CreatedAt = DateTime.UtcNow;

            var startDate = DateTime.UtcNow;
            var endDate = subscriptionType == SubscriptionType.Monthly
                ? startDate.AddMonths(1).AddDays(-1)
                : startDate.AddYears(1).AddDays(-1);

            SubscriptionPeriod = new DateRange(startDate, endDate);
        }

        public bool IsActive => Status == SubscriptionStatus.Active && SubscriptionPeriod.Contains(DateTime.UtcNow);

        public void ApplyDiscount(Money discountAmount, string updatedBy)
        {
            if (discountAmount.Currency != Amount.Currency)
                throw new ArgumentException("Discount currency must match subscription currency");

            DiscountAmount = discountAmount;
            UpdatedBy = updatedBy;
            UpdateTimestamp();
        }

        public void Renew(string updatedBy)
        {
            var newStartDate = SubscriptionPeriod.EndDate.AddDays(1);
            var newEndDate = SubscriptionType == SubscriptionType.Monthly
                ? newStartDate.AddMonths(1).AddDays(-1)
                : newStartDate.AddYears(1).AddDays(-1);

            SubscriptionPeriod = new DateRange(newStartDate, newEndDate);
            Status = SubscriptionStatus.Active;
            UpdatedBy = updatedBy;
            UpdateTimestamp();
        }

        public void Cancel(string updatedBy)
        {
            Status = SubscriptionStatus.Cancelled;
            AutoRenew = false;
            UpdatedBy = updatedBy;
            UpdateTimestamp();
        }

        public void Expire(string updatedBy)
        {
            Status = SubscriptionStatus.Expired;
            UpdatedBy = updatedBy;
            UpdateTimestamp();
        }

        public void EnableAutoRenew(string updatedBy)
        {
            AutoRenew = true;
            UpdatedBy = updatedBy;
            UpdateTimestamp();
        }

        public void DisableAutoRenew(string updatedBy)
        {
            AutoRenew = false;
            UpdatedBy = updatedBy;
            UpdateTimestamp();
        }

        public Money GetEffectiveAmount()
        {
            return DiscountAmount != null ? Amount.Subtract(DiscountAmount) : Amount;
        }
    }
}
