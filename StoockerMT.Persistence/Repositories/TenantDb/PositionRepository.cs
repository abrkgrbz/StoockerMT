using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoockerMT.Domain.Entities.TenantDb;
using StoockerMT.Domain.Repositories.TenantDb;
using StoockerMT.Persistence.Contexts;
using StoockerMT.Persistence.Repositories.Common;

namespace StoockerMT.Persistence.Repositories.TenantDb
{
    public class PositionRepository:RepositoryBase<Position>, IPositionRepository
    {
        private readonly TenantDbContext _context;

        public PositionRepository(TenantDbContext context):base(context)
        {
            _context = context;
        }

        public async Task<Position?> GetByTitleAsync(string title, CancellationToken cancellationToken = default)
        {
            return await _context.Positions
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Title == title, cancellationToken);
        }

        public async Task<Position?> GetWithEmployeesAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Positions
                .AsNoTracking()
                .Include(p => p.Employees)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Position>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Positions
                .AsNoTracking()
                .Where(p => p.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Position>> GetBySalaryRangeAsync(decimal minSalary, decimal maxSalary, CancellationToken cancellationToken = default)
        {
            return await _context.Positions
                .AsNoTracking()
                .Where(p => p.MinSalary >= minSalary && p.MaxSalary <= maxSalary)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsByTitleAsync(string title, CancellationToken cancellationToken = default)
        {
            return await _context.Positions
                .AsNoTracking()
                .AnyAsync(p => p.Title == title, cancellationToken);
        }

        public async Task<int> GetEmployeeCountAsync(int positionId, CancellationToken cancellationToken = default)
        {
            return await _context.Positions
                .AsNoTracking()
                .Where(p => p.Id == positionId)
                .Select(p => p.Employees.Count)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
