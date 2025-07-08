using StoockerMT.Domain.Entities.MasterDb;
using StoockerMT.Domain.Enums;
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
    public class TenantRepository : RepositoryBase<Tenant>, ITenantRepository
    {
        private readonly MasterDbContext _context;

        public TenantRepository(MasterDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Tenant?> GetByCodeAsync(TenantCode code, CancellationToken cancellationToken = default)
        {
            return await _context.Tenants
                .FirstOrDefaultAsync(t => t.Code.Value == code.Value, cancellationToken);
        }

        public async Task<Tenant?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Tenants
                .FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
        }

        public async Task<Tenant?> GetWithModulesAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Tenants
                .Include(t => t.ModuleSubscriptions)
                    .ThenInclude(s => s.Module)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task<Tenant?> GetWithUsersAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Tenants
                .Include(t => t.Users)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Tenant>> GetByStatusAsync(TenantStatus status, CancellationToken cancellationToken = default)
        {
            return await _context.Tenants
                .Where(t => t.Status == status)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Tenant>> GetActiveTenantsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Tenants
                .Where(t => t.Status == TenantStatus.Active)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsByCodeAsync(TenantCode code, CancellationToken cancellationToken = default)
        {
            return await _context.Tenants
                .AnyAsync(t => t.Code.Value == code.Value, cancellationToken);
        }

        public async Task<bool> IsConnectionStringUniqueAsync(string connectionString, int? excludeTenantId = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Tenants.AsQueryable();

            if (excludeTenantId.HasValue)
            {
                query = query.Where(t => t.Id != excludeTenantId.Value);
            }

            return !await query.AnyAsync(t => t.DatabaseInfo.GetDecryptedConnectionString() == connectionString, cancellationToken);
        }
    }
}
