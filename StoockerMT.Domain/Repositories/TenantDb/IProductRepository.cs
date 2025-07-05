using StoockerMT.Domain.Entities.TenantDb;
using StoockerMT.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Repositories.TenantDb
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
        Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
        Task<Product?> GetWithMovementsAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Product>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Product>> GetLowStockProductsAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Product>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Product>> GetByPriceRangeAsync(Money minPrice, Money maxPrice, CancellationToken cancellationToken = default);
        Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
        Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default);
    }
}
