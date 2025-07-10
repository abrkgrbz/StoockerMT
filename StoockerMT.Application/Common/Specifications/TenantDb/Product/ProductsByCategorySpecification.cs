using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Domain.Specifications;

namespace StoockerMT.Application.Common.Specifications.TenantDb.Product
{
    public class ProductsByCategorySpecification : BaseSpecification<Domain.Entities.TenantDb.Product>
    {
        public ProductsByCategorySpecification(int categoryId, bool includeSubcategories = false)
        {
            if (includeSubcategories)
            {
                AddInclude(p => p.Category);
                Criteria = p => p.CategoryId == categoryId ||
                                p.Category.ParentCategoryId == categoryId;
            }
            else
            {
                Criteria = p => p.CategoryId == categoryId;
            }

            Criteria = Criteria.And(p => p.IsActive && !p.IsDeleted);
            ApplyOrderBy(p => p.ProductName);
        }
    }

}
