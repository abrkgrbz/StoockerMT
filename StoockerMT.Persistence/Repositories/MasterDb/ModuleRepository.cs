using StoockerMT.Domain.Enums;
using StoockerMT.Domain.Repositories.MasterDb;
using StoockerMT.Persistence.Contexts;
using StoockerMT.Persistence.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoockerMT.Domain.Entities.MasterDb;

namespace StoockerMT.Persistence.Repositories.MasterDb
{
    public class ModuleRepository : RepositoryBase<Module>, IModuleRepository
    {
        private readonly MasterDbContext _context;

        public ModuleRepository(MasterDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Module?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _context.Modules
                .FirstOrDefaultAsync(m => m.Code == code, cancellationToken);
        }

        public async Task<Module?> GetWithPermissionsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Modules
                .Include(m => m.Permissions)
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        }

        public async Task<Module?> GetWithFeaturesAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Modules
                .Include(m => m.Features)
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Module>> GetByCategoryAsync(ModuleCategory category, CancellationToken cancellationToken = default)
        {
            return await _context.Modules
                .Where(m => m.Category == category)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Module>> GetActiveModulesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Modules
                .Where(m => m.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Module>> GetByTenantAsync(int tenantId, CancellationToken cancellationToken = default)
        {
            return await _context.TenantModuleSubscriptions
                .Where(s => s.TenantId == tenantId && s.Status == SubscriptionStatus.Active)
                .Select(s => s.Module)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _context.Modules
                .AnyAsync(m => m.Code == code, cancellationToken);
        }
    }
}
