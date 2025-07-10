using StoockerMT.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Specifications.TenantDb.Product
{
    public class ProductSearchSpecification : BaseSpecification<Domain.Entities.TenantDb.Product>
    {
        public ProductSearchSpecification(
            string searchTerm = null,
            int? categoryId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool? inStock = null,
            string sortBy = "name",
            bool descending = false,
            int? pageNumber = null,
            int? pageSize = null)
        {
            // Base criteria
            Criteria = p => p.IsActive && !p.IsDeleted;

            // Search term
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                Criteria = Criteria.And(p =>
                    p.ProductName.ToLower().Contains(searchTerm) ||
                    p.ProductCode.ToLower().Contains(searchTerm) ||
                    p.SKU.ToLower().Contains(searchTerm) ||
                    (p.Description != null && p.Description.ToLower().Contains(searchTerm)));
            }

            // Category filter
            if (categoryId.HasValue)
            {
                Criteria = Criteria.And(p => p.CategoryId == categoryId.Value);
            }

            // Price range filter
            if (minPrice.HasValue)
            {
                Criteria = Criteria.And(p => p.UnitPrice.Amount >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                Criteria = Criteria.And(p => p.UnitPrice.Amount <= maxPrice.Value);
            }

            // Stock filter
            if (inStock.HasValue)
            {
                if (inStock.Value)
                    Criteria = Criteria.And(p => p.StockQuantity.Value > 0);
                else
                    Criteria = Criteria.And(p => p.StockQuantity.Value == 0);
            }

            // Sorting
            switch (sortBy.ToLower())
            {
                case "price":
                    if (descending)
                        ApplyOrderByDescending(p => p.UnitPrice.Amount);
                    else
                        ApplyOrderBy(p => p.UnitPrice.Amount);
                    break;
                case "stock":
                    if (descending)
                        ApplyOrderByDescending(p => p.StockQuantity.Value);
                    else
                        ApplyOrderBy(p => p.StockQuantity.Value);
                    break;
                case "created":
                    if (descending)
                        ApplyOrderByDescending(p => p.CreatedAt);
                    else
                        ApplyOrderBy(p => p.CreatedAt);
                    break;
                default: // name
                    if (descending)
                        ApplyOrderByDescending(p => p.ProductName);
                    else
                        ApplyOrderBy(p => p.ProductName);
                    break;
            }

            // Include category for display
            AddInclude(p => p.Category);

            // Paging
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                ApplyPaging((pageNumber.Value - 1) * pageSize.Value, pageSize.Value);
            }

            // No tracking for search
            ApplyNoTracking();
        }
    }
}
