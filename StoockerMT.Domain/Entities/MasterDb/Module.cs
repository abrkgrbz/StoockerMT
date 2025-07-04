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
    public class Module : BaseEntity, IAuditableEntity
    {
        public string Name { get; private set; }
        public string Code { get; private set; }
        public string? Description { get; private set; }
        public Money MonthlyPrice { get; private set; }
        public Money YearlyPrice { get; private set; }
        public string Version { get; private set; } = "1.0.0";
        public bool IsActive { get; private set; } = true;
        public ModuleCategory Category { get; private set; }
        public string CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Module specific settings
        public string? Configuration { get; private set; }
        public string? Dependencies { get; private set; }

        // Navigation Properties
        public virtual ICollection<TenantModuleSubscription> TenantSubscriptions { get; set; } = new List<TenantModuleSubscription>();
        public virtual ICollection<ModulePermission> Permissions { get; set; } = new List<ModulePermission>();
        public virtual ICollection<ModuleFeature> Features { get; set; } = new List<ModuleFeature>();

        private Module() { }

        public Module(string name, string code, Money monthlyPrice, Money yearlyPrice, ModuleCategory category, string createdBy)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Code = code ?? throw new ArgumentNullException(nameof(code));
            MonthlyPrice = monthlyPrice ?? throw new ArgumentNullException(nameof(monthlyPrice));
            YearlyPrice = yearlyPrice ?? throw new ArgumentNullException(nameof(yearlyPrice));

            if (monthlyPrice.Currency != yearlyPrice.Currency)
                throw new ArgumentException("Monthly and yearly prices must have the same currency");

            Category = category;
            CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdatePricing(Money monthlyPrice, Money yearlyPrice, string updatedBy)
        {
            if (monthlyPrice.Currency != yearlyPrice.Currency)
                throw new ArgumentException("Monthly and yearly prices must have the same currency");

            MonthlyPrice = monthlyPrice ?? throw new ArgumentNullException(nameof(monthlyPrice));
            YearlyPrice = yearlyPrice ?? throw new ArgumentNullException(nameof(yearlyPrice));
            UpdatedBy = updatedBy;
            UpdateTimestamp();
        }

        public void ApplyDiscount(Percentage discount, string updatedBy)
        {
            MonthlyPrice = MonthlyPrice.ApplyDiscount(discount);
            YearlyPrice = YearlyPrice.ApplyDiscount(discount);
            UpdatedBy = updatedBy;
            UpdateTimestamp();
        }

        public void UpdateConfiguration(string configuration, string updatedBy)
        {
            Configuration = configuration;
            UpdatedBy = updatedBy;
            UpdateTimestamp();
        }

        public void UpdateDescription(string description)
        {
            Description = description;
            UpdateTimestamp();
        }

        public void UpdateVersion(string version)
        {
            Version = version ?? "1.0.0";
            UpdateTimestamp();
        }

        public void Activate()
        {
            IsActive = true;
            UpdateTimestamp();
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdateTimestamp();
        }
        public void Deleted()
        {
            IsDeleted = true;
            UpdateTimestamp();
        }
    }

}
