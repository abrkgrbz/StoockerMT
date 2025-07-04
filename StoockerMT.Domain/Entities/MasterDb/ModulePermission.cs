using StoockerMT.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Domain.Enums;

namespace StoockerMT.Domain.Entities.MasterDb
{
    public class ModulePermission : BaseEntity
    {
        public int ModuleId { get; private set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; private set; }

        [Required]
        [MaxLength(100)]
        public string Code { get; private set; }

        [MaxLength(500)]
        public string? Description { get; private set; }

        public PermissionType Type { get; private set; }

        // Navigation Properties
        [ForeignKey("ModuleId")]
        public virtual Module Module { get; set; }

        public virtual ICollection<TenantUserPermission> UserPermissions { get; set; } = new List<TenantUserPermission>();

        private ModulePermission() { }

        public ModulePermission(int moduleId, string name, string code, PermissionType type, string? description = null)
        {
            ModuleId = moduleId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Type = type;
            Description = description;
        }
    }
}
