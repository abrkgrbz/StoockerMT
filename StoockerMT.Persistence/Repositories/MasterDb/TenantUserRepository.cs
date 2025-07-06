using StoockerMT.Domain.Entities.MasterDb;
using StoockerMT.Domain.Repositories.MasterDb;
using StoockerMT.Domain.ValueObjects;
using StoockerMT.Persistence.Contexts;
using StoockerMT.Persistence.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace StoockerMT.Persistence.Repositories.MasterDb
{
    public class TenantUserRepository : RepositoryBase<TenantUser>, ITenantUserRepository
    {
        private readonly MasterDbContext _context;

        public TenantUserRepository(MasterDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<TenantUser?> GetByEmailAsync(Email email, int tenantId, CancellationToken cancellationToken = default)
        {
            return await _context.TenantUsers
                .FirstOrDefaultAsync(u => u.Email.Value == email.Value && u.TenantId == tenantId, cancellationToken);
        }

        public async Task<TenantUser?> GetWithPermissionsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.TenantUsers
                .Include(u => u.Permissions)
                    .ThenInclude(p => p.Permission)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<TenantUser>> GetByTenantAsync(int tenantId, CancellationToken cancellationToken = default)
        {
            return await _context.TenantUsers
                .Where(u => u.TenantId == tenantId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<TenantUser>> GetAdminsByTenantAsync(int tenantId, CancellationToken cancellationToken = default)
        {
            return await _context.TenantUsers
                .Where(u => u.TenantId == tenantId && u.IsTenantAdmin)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<TenantUser>> GetActiveUsersByTenantAsync(int tenantId, CancellationToken cancellationToken = default)
        {
            return await _context.TenantUsers
                .Where(u => u.TenantId == tenantId && u.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsByEmailAsync(Email email, int tenantId, CancellationToken cancellationToken = default)
        {
            return await _context.TenantUsers
                .AnyAsync(u => u.Email.Value == email.Value && u.TenantId == tenantId, cancellationToken);
        }

        public async Task<int> GetUserCountByTenantAsync(int tenantId, CancellationToken cancellationToken = default)
        {
            return await _context.TenantUsers
                .CountAsync(u => u.TenantId == tenantId && u.IsActive, cancellationToken);
        }
    }
}
