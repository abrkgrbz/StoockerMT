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
    public class DepartmentRepository : RepositoryBase<Department>, IDepartmentRepository
    {
        private readonly TenantDbContext _context;

        public DepartmentRepository(TenantDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Department?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.DepartmentName == name, cancellationToken);
        }

        public async Task<Department?> GetWithEmployeesAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Departments
                .AsNoTracking()
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Department>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Departments
                .AsNoTracking()
                .Where(d => d.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Departments
                .AsNoTracking()
                .AnyAsync(d => d.DepartmentName == name, cancellationToken);
        }

        public async Task<int> GetEmployeeCountAsync(int departmentId, CancellationToken cancellationToken = default)
        {
            return await _context.Employees
                .AsNoTracking()
                .CountAsync(e => e.DepartmentId == departmentId, cancellationToken);
        }
    }
}
