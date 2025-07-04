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
    public class ModuleFeature : BaseEntity
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

        public bool IsEnabled { get; private set; } = true;

        // Feature configuration (JSON)
        public string? Configuration { get; private set; }

        // Navigation Properties
        [ForeignKey("ModuleId")]
        public virtual Module Module { get; set; }

        private ModuleFeature() { }

        public ModuleFeature(int moduleId, string name, string code, string? description = null)
        {
            ModuleId = moduleId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Description = description;
        }
    }

}
