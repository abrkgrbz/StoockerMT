using StoockerMT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Domain.Entities.MasterDb;

namespace StoockerMT.Domain.Repositories.MasterDb
{
    public interface IModuleRepository : IRepository<Module>
    {
        Task<Module?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
        Task<Module?> GetWithPermissionsAsync(int id, CancellationToken cancellationToken = default);
        Task<Module?> GetWithFeaturesAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Module>> GetByCategoryAsync(ModuleCategory category, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Module>> GetActiveModulesAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Module>> GetByTenantAsync(int tenantId, CancellationToken cancellationToken = default);
        Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
    }
}
