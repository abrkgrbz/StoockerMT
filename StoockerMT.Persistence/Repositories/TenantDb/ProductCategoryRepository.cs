using StoockerMT.Domain.Entities.TenantDb;
using StoockerMT.Domain.Repositories.TenantDb;
using StoockerMT.Persistence.Contexts;
using StoockerMT.Persistence.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace StoockerMT.Persistence.Repositories.TenantDb
{
    public class ProductCategoryRepository : RepositoryBase<ProductCategory>, IProductCategoryRepository
    {
        private readonly TenantDbContext _context;

        public ProductCategoryRepository(TenantDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ProductCategory?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.ProductCategories
                .FirstOrDefaultAsync(pc => pc.CategoryName == name, cancellationToken);
        }

        public async Task<ProductCategory?> GetWithProductsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.ProductCategories
                .Include(pc => pc.Products)
                .FirstOrDefaultAsync(pc => pc.Id == id, cancellationToken);
        }

        public async Task<ProductCategory?> GetWithSubCategoriesAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.ProductCategories
                .Include(pc => pc.SubCategories)
                .FirstOrDefaultAsync(pc => pc.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<ProductCategory>> GetRootCategoriesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.ProductCategories
                .Where(pc => pc.ParentCategoryId == null)
                .OrderBy(pc => pc.CategoryName)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<ProductCategory>> GetSubCategoriesAsync(int parentCategoryId, CancellationToken cancellationToken = default)
        {
            return await _context.ProductCategories
                .Where(pc => pc.ParentCategoryId == parentCategoryId)
                .OrderBy(pc => pc.CategoryName)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<ProductCategory>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            return await _context.ProductCategories
                .Where(pc => pc.IsActive)
                .OrderBy(pc => pc.CategoryName)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.ProductCategories
                .AnyAsync(pc => pc.CategoryName == name, cancellationToken);
        }

        public async Task<int> GetProductCountAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .CountAsync(p => p.CategoryId == categoryId, cancellationToken);
        }
    }
}
