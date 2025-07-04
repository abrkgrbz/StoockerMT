using StoockerMT.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Domain.ValueObjects;

namespace StoockerMT.Domain.Entities.MasterDb
{
    public class TenantUser : BaseEntity, IAuditableEntity
    {
        public int TenantId { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public Email Email { get; private set; }
        public string PasswordHash { get; private set; }
        public PhoneNumber? PhoneNumber { get; private set; }
        public bool IsActive { get; private set; } = true;
        public bool IsTenantAdmin { get; private set; } = false;
        public DateTime? LastLoginAt { get; private set; }
        public DateTime? EmailVerifiedAt { get; private set; }
        public string CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public virtual Tenant Tenant { get; set; }
        public virtual ICollection<TenantUserPermission> Permissions { get; set; } = new List<TenantUserPermission>();

        private TenantUser() { }

        public TenantUser(int tenantId, string firstName, string lastName, Email email, string passwordHash, string createdBy)
        {
            TenantId = tenantId;
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
            CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));
            CreatedAt = DateTime.UtcNow;
        }

        public string FullName => $"{FirstName} {LastName}";

        public void UpdateProfile(string firstName, string lastName, PhoneNumber? phoneNumber, string updatedBy)
        {
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            PhoneNumber = phoneNumber;
            UpdatedBy = updatedBy;
            UpdateTimestamp();
        }

        public void UpdateEmail(Email email, string updatedBy)
        {
            Email = email ?? throw new ArgumentNullException(nameof(email));
            EmailVerifiedAt = null; // Reset verification
            UpdatedBy = updatedBy;
            UpdateTimestamp();
        }

        public void UpdatePassword(string passwordHash, string updatedBy)
        {
            PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
            UpdatedBy = updatedBy;
            UpdateTimestamp();
        }

        public void SetAsAdmin(string updatedBy)
        {
            IsTenantAdmin = true;
            UpdatedBy = updatedBy;
            UpdateTimestamp();
        }

        public void RemoveAdmin(string updatedBy)
        {
            IsTenantAdmin = false;
            UpdatedBy = updatedBy;
            UpdateTimestamp();
        }

        public void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
            UpdateTimestamp();
        }

        public void VerifyEmail()
        {
            EmailVerifiedAt = DateTime.UtcNow;
            UpdateTimestamp();
        }

        public void Activate(string updatedBy)
        {
            IsActive = true;
            UpdatedBy = updatedBy;
            UpdateTimestamp();
        }

        public void Deactivate(string updatedBy)
        {
            IsActive = false;
            UpdatedBy = updatedBy;
            UpdateTimestamp();
        }
    }

}
