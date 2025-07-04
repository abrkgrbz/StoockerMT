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
    public class ProductCategory : TenantBaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string CategoryName { get; private set; }

        [MaxLength(500)]
        public string? Description { get; private set; }

        public int? ParentCategoryId { get; private set; }

        public bool IsActive { get; private set; } = true;

        // Navigation Properties
        [ForeignKey("ParentCategoryId")]
        public virtual ProductCategory ParentCategory { get; set; }

        public virtual ICollection<ProductCategory> SubCategories { get; set; } = new List<ProductCategory>();
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();

        private ProductCategory() { }

        public ProductCategory(string categoryName, int? parentCategoryId = null)
        {
            CategoryName = categoryName ?? throw new ArgumentNullException(nameof(categoryName));
            ParentCategoryId = parentCategoryId;
        }
    }
}
