using Microsoft.EntityFrameworkCore;
using StoockerMT.Domain.Entities.TenantDb;
using StoockerMT.Domain.Repositories.TenantDb;
using StoockerMT.Domain.ValueObjects;
using StoockerMT.Persistence.Contexts;
using StoockerMT.Persistence.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Persistence.Repositories.TenantDb
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        private readonly TenantDbContext _context;

        public ProductRepository(TenantDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Product?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.ProductCode == code, cancellationToken);
        }

        public async Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.SKU == sku, cancellationToken);
        }

        public async Task<Product?> GetWithMovementsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Include(p => p.InventoryMovements)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Product>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Where(p => p.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Product>> GetLowStockProductsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Where(p => p.StockQuantity.Value <= p.MinimumStockLevel.Value)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Product>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Where(p => EF.Functions.Like(p.ProductName, $"%{searchTerm}%"))
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Product>> GetByPriceRangeAsync(Money minPrice, Money maxPrice, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Where(p =>
                    p.UnitPrice.Currency == minPrice.Currency &&
                    p.UnitPrice.Amount >= minPrice.Amount &&
                    p.UnitPrice.Amount <= maxPrice.Amount)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .AnyAsync(p => p.ProductCode == code, cancellationToken);
        }

        public async Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .AnyAsync(p => p.SKU == sku, cancellationToken);
        }
    }
}
