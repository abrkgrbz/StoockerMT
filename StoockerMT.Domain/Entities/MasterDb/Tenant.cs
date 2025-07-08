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
        // Basic Properties
        public string Name { get; private set; }
        public TenantCode Code { get; private set; }
        public string? Description { get; private set; }
        public TenantStatus Status { get; private set; } = TenantStatus.Pending;

        // Audit Properties
        public string CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Value Objects
        public TenantSettings? Settings { get; private set; }
        public DatabaseInfo? DatabaseInfo { get; private set; }

        // Subscription Limits
        public int MaxUsers { get; private set; } = 10;
        public long MaxStorageBytes { get; private set; } = 1073741824; // 1GB
        public int MaxModules { get; private set; } = 5;

        // Status Dates
        public DateTime? ActivatedDate { get; private set; }
        public DateTime? DeactivatedDate { get; private set; }
        public string? DeactivationReason { get; private set; }

        // Navigation Properties
        public virtual ICollection<TenantModuleSubscription> ModuleSubscriptions { get; private set; } = new List<TenantModuleSubscription>();
        public virtual ICollection<TenantUser> Users { get; private set; } = new List<TenantUser>();
        public virtual ICollection<TenantInvoice> Invoices { get; private set; } = new List<TenantInvoice>();

        // Private constructor for EF Core
        private Tenant() { }

        // Public constructor for creating new tenant
        public Tenant(string name, TenantCode code, string createdBy, string? description = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Description = description;
            CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));
            CreatedAt = DateTime.UtcNow;
            Status = TenantStatus.Pending;

            // Initialize with default settings
            Settings = TenantSettings.CreateDefault();
        }

        // Database Management
        public void SetDatabaseInfo(DatabaseInfo databaseInfo)
        {
            DatabaseInfo = databaseInfo ?? throw new ArgumentNullException(nameof(databaseInfo));
            UpdateTimestamp();
        }

        public void ClearDatabaseInfo()
        {
            DatabaseInfo = null;
            UpdateTimestamp();
        }

        // Settings Management
        public void UpdateSettings(TenantSettings settings)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            UpdateTimestamp();
        }

        // Basic Information Updates
        public void UpdateName(string name, string updatedBy)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            UpdatedBy = updatedBy;
            UpdateTimestamp();
        }

        public void UpdateDescription(string? description, string updatedBy)
        {
            Description = description;
            UpdatedBy = updatedBy;
            UpdateTimestamp();
        }

        // Limits Management
        public void UpdateLimits(int? maxUsers = null, long? maxStorageBytes = null, int? maxModules = null, string updatedBy = null)
        {
            if (maxUsers.HasValue)
            {
                if (maxUsers.Value <= 0)
                    throw new ArgumentException("Max users must be greater than zero", nameof(maxUsers));
                MaxUsers = maxUsers.Value;
            }

            if (maxStorageBytes.HasValue)
            {
                if (maxStorageBytes.Value <= 0)
                    throw new ArgumentException("Max storage must be greater than zero", nameof(maxStorageBytes));
                MaxStorageBytes = maxStorageBytes.Value;
            }

            if (maxModules.HasValue)
            {
                if (maxModules.Value <= 0)
                    throw new ArgumentException("Max modules must be greater than zero", nameof(maxModules));
                MaxModules = maxModules.Value;
            }

            if (!string.IsNullOrEmpty(updatedBy))
                UpdatedBy = updatedBy;

            UpdateTimestamp();
        }

        // Status Management
        public void Activate(string updatedBy)
        {
            if (Status == TenantStatus.Terminated)
                throw new InvalidOperationException("Cannot activate a terminated tenant");

            if (DatabaseInfo == null)
                throw new InvalidOperationException("Cannot activate tenant without database information");

            Status = TenantStatus.Active;
            ActivatedDate = DateTime.UtcNow;
            DeactivatedDate = null;
            DeactivationReason = null;
            UpdatedBy = updatedBy;
            UpdateTimestamp();
        }

        public void Deactivate(string reason, string updatedBy)
        {
            if (Status == TenantStatus.Terminated)
                throw new InvalidOperationException("Cannot deactivate a terminated tenant");

            Status = TenantStatus.Inactive;
            DeactivatedDate = DateTime.UtcNow;
            DeactivationReason = reason;
            UpdatedBy = updatedBy;
            UpdateTimestamp();
        }

        public void Suspend(string reason, string updatedBy)
        {
            if (Status == TenantStatus.Terminated)
                throw new InvalidOperationException("Cannot suspend a terminated tenant");

            Status = TenantStatus.Suspended;
            DeactivatedDate = DateTime.UtcNow;
            DeactivationReason = reason;
            UpdatedBy = updatedBy;
            UpdateTimestamp();
        }

        public void Terminate(string reason, string updatedBy)
        {
            Status = TenantStatus.Terminated;
            DeactivatedDate = DateTime.UtcNow;
            DeactivationReason = reason;
            UpdatedBy = updatedBy;
            UpdateTimestamp();
        }

        // Module Subscription Management
        public void AddModuleSubscription(TenantModuleSubscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription));

            if (ModuleSubscriptions.Count >= MaxModules)
                throw new InvalidOperationException($"Cannot add more modules. Maximum limit of {MaxModules} reached.");

            if (ModuleSubscriptions.Any(s => s.ModuleId == subscription.ModuleId && s.Status == SubscriptionStatus.Active))
                throw new InvalidOperationException("An active subscription for this module already exists");

            ModuleSubscriptions.Add(subscription);
            UpdateTimestamp();
        }

        public void RemoveModuleSubscription(int moduleId)
        {
            var subscription = ModuleSubscriptions.FirstOrDefault(s => s.ModuleId == moduleId);
            if (subscription != null)
            {
                ModuleSubscriptions.Remove(subscription);
                UpdateTimestamp();
            }
        }

        // User Management
        public void AddUser(TenantUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var activeUserCount = Users.Count(u => u.IsActive);
            if (activeUserCount >= MaxUsers)
                throw new InvalidOperationException($"Cannot add more users. Maximum limit of {MaxUsers} reached.");

            Users.Add(user);
            UpdateTimestamp();
        }

        public void RemoveUser(int userId)
        {
            var user = Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                Users.Remove(user);
                UpdateTimestamp();
            }
        }

        // Query Methods
        public bool IsActive() => Status == TenantStatus.Active;

        public bool CanAccessModule(string moduleCode)
        {
            if (!IsActive()) return false;

            return ModuleSubscriptions.Any(s =>
                s.Module?.Code == moduleCode &&
                s.Status == SubscriptionStatus.Active &&
                s.IsWithinSubscriptionPeriod());
        }

        public bool HasDatabase() => DatabaseInfo != null;

        public bool IsDatabaseHealthy() => DatabaseInfo?.IsHealthy() ?? false;

        public int GetActiveUserCount() => Users.Count(u => u.IsActive);

        public int GetActiveModuleCount() => ModuleSubscriptions.Count(s => s.Status == SubscriptionStatus.Active);

        public long GetCurrentStorageUsage()
        {
            // This would typically come from a storage service
            // For now, return database size if available
            return (DatabaseInfo?.SizeInMB ?? 0) * 1024 * 1024; // Convert MB to bytes
        }

        public bool IsStorageLimitExceeded()
        {
            return GetCurrentStorageUsage() > MaxStorageBytes;
        }

        // Domain Events (if using domain events)
        public void RaiseTenantActivatedEvent()
        {
            // AddDomainEvent(new TenantActivatedEvent(Id, Code.Value, Name));
        }

        public void RaiseTenantDeactivatedEvent()
        {
            // AddDomainEvent(new TenantDeactivatedEvent(Id, Code.Value, DeactivationReason));
        }

        // Private helper methods
        private void UpdateTimestamp()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
