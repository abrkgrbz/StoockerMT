using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoockerMT.Domain.Entities.MasterDb;
using StoockerMT.Domain.Repositories.MasterDb;
using StoockerMT.Persistence.Contexts;
using StoockerMT.Persistence.Repositories.Common;

namespace StoockerMT.Persistence.Repositories.MasterDb
{
    public class ModulePermissionRepository : RepositoryBase<ModulePermission>, IModulePermissionRepository
    {
        private readonly MasterDbContext _context;

        public ModulePermissionRepository(MasterDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<ModulePermission>> GetByModuleAsync(int moduleId, CancellationToken cancellationToken = default)
        {
            return await _context.ModulePermissions
                .Where(p => p.ModuleId == moduleId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<ModulePermission>> GetByUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.TenantUserPermissions
                .Where(up => up.TenantUserId == userId && up.IsActive)
                .Select(up => up.Permission)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> UserHasPermissionAsync(int userId, int moduleId, string permissionCode, CancellationToken cancellationToken = default)
        {
            return await _context.TenantUserPermissions
                .AnyAsync(up =>
                        up.TenantUserId == userId &&
                        up.Permission.ModuleId == moduleId &&
                        up.Permission.Code == permissionCode &&
                        up.IsActive,
                    cancellationToken);
        }
    }
}
