using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Domain.Entities.TenantDb.Common;

namespace StoockerMT.Domain.Entities.TenantDb
{
    public class Position : TenantBaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; private set; }

        [MaxLength(1000)]
        public string? Description { get; private set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MinSalary { get; private set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MaxSalary { get; private set; }

        public bool IsActive { get; private set; } = true;

        // Navigation Properties
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

        private Position() { }

        public Position(string title, decimal minSalary, decimal maxSalary)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            MinSalary = minSalary;
            MaxSalary = maxSalary;
        }
    }
}
