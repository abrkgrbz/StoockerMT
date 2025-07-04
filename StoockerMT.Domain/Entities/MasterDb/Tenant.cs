using StoockerMT.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Domain.Enums;
using StoockerMT.Domain.ValueObjects;

namespace StoockerMT.Domain.Entities.MasterDb
{
    public class Tenant : BaseEntity, IAuditableEntity
    {
        public string Name { get; private set; }
        public TenantCode Code { get; private set; }
        public string DatabaseName { get; private set; }
        public string ConnectionString { get; private set; }
        public string? Description { get; private set; }
        public TenantStatus Status { get; private set; } = TenantStatus.Active;
        public string CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Configuration JSON
        public string? Settings { get; private set; }

        // Subscription limits
        public int MaxUsers { get; private set; } = 10;
        public long MaxStorageBytes { get; private set; } = 1073741824; // 1GB

        // Navigation Properties
        public virtual ICollection<TenantModuleSubscription> ModuleSubscriptions { get; set; } = new List<TenantModuleSubscription>();
        public virtual ICollection<TenantUser> Users { get; set; } = new List<TenantUser>();
        public virtual ICollection<TenantInvoice> Invoices { get; set; } = new List<TenantInvoice>();

        private Tenant() { }

        public Tenant(string name, TenantCode code, string createdBy)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Code = code ?? throw new ArgumentNullException(nameof(code));
            DatabaseName = $"TenantDB_{code.Value}";
            ConnectionString = string.Empty; // Will be set after database creation
            CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateConnectionString(string connectionString)
        {
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            UpdateTimestamp();
        }

        public void UpdateName(string name, string updatedBy)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            UpdatedBy = updatedBy;
            UpdateTimestamp();
        }

        public void UpdateSettings(string settings, string updatedBy)
        {
            Settings = settings;
            UpdatedBy = updatedBy;
            UpdateTimestamp();
        }

        public void UpdateLimits(int maxUsers, long maxStorageBytes, string updatedBy)
        {
            if (maxUsers <= 0)
                throw new ArgumentException("Max users must be greater than zero", nameof(maxUsers));

            if (maxStorageBytes <= 0)
                throw new ArgumentException("Max storage must be greater than zero", nameof(maxStorageBytes));

            MaxUsers = maxUsers;
            MaxStorageBytes = maxStorageBytes;
            UpdatedBy = updatedBy;
            UpdateTimestamp();
        }

        public void Activate(string updatedBy)
        {
            Status = TenantStatus.Active;
            UpdatedBy = updatedBy;
            UpdateTimestamp();
        }

        public void Deactivate(string updatedBy)
        {
            Status = TenantStatus.Inactive;
            UpdatedBy = updatedBy;
            UpdateTimestamp();
        }

        public void Suspend(string updatedBy)
        {
            Status = TenantStatus.Suspended;
            UpdatedBy = updatedBy;
            UpdateTimestamp();
        }

        public void Terminate(string updatedBy)
        {
            Status = TenantStatus.Terminated;
            UpdatedBy = updatedBy;
            UpdateTimestamp();
        }
    }
}
