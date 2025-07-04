using StoockerMT.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Entities.MasterDb
{
    public class TenantUserPermission : BaseEntity
    {
        public int TenantUserId { get; private set; }
        public int PermissionId { get; private set; }

        public DateTime GrantedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; private set; }

        [MaxLength(100)]
        public string GrantedBy { get; private set; }

        // Navigation Properties
        [ForeignKey("TenantUserId")]
        public virtual TenantUser TenantUser { get; set; }

        [ForeignKey("PermissionId")]
        public virtual ModulePermission Permission { get; set; }

        private TenantUserPermission() { }

        public TenantUserPermission(int tenantUserId, int permissionId, string grantedBy, DateTime? expiresAt = null)
        {
            TenantUserId = tenantUserId;
            PermissionId = permissionId;
            GrantedBy = grantedBy ?? throw new ArgumentNullException(nameof(grantedBy));
            ExpiresAt = expiresAt;
        }

        public bool IsActive => ExpiresAt == null || DateTime.UtcNow <= ExpiresAt;
    }
}
