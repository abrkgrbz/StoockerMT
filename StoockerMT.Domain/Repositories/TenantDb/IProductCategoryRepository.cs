using StoockerMT.Domain.Entities.TenantDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Repositories.TenantDb
{
    public interface IProductCategoryRepository : IRepository<ProductCategory>
    {
        Task<ProductCategory?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<ProductCategory?> GetWithProductsAsync(int id, CancellationToken cancellationToken = default);
        Task<ProductCategory?> GetWithSubCategoriesAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ProductCategory>> GetRootCategoriesAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ProductCategory>> GetSubCategoriesAsync(int parentCategoryId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ProductCategory>> GetActiveAsync(CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<int> GetProductCountAsync(int categoryId, CancellationToken cancellationToken = default);
    }
}
