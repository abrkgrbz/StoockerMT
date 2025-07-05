using StoockerMT.Domain.Entities.MasterDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Repositories.MasterDb
{
    public interface IModulePermissionRepository : IRepository<ModulePermission>
    {
        Task<IReadOnlyList<ModulePermission>> GetByModuleAsync(int moduleId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ModulePermission>> GetByUserAsync(int userId, CancellationToken cancellationToken = default);
        Task<bool> UserHasPermissionAsync(int userId, int moduleId, string permissionCode, CancellationToken cancellationToken = default);
    }
}
