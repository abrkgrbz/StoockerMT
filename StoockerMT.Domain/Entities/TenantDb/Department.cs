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
    public class Department : TenantBaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string DepartmentName { get; private set; }

        [MaxLength(500)]
        public string? Description { get; private set; }

        public int? ManagerId { get; private set; }

        public bool IsActive { get; private set; } = true;

        // Navigation Properties
        [ForeignKey("ManagerId")]
        public virtual Employee Manager { get; set; }

        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

        private Department() { }

        public Department(string departmentName, int? managerId = null)
        {
            DepartmentName = departmentName ?? throw new ArgumentNullException(nameof(departmentName));
            ManagerId = managerId;
        }
    }
}
