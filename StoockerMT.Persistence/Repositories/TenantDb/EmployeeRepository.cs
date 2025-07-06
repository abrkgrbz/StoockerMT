using StoockerMT.Domain.Entities.TenantDb;
using StoockerMT.Domain.Repositories.TenantDb;
using StoockerMT.Persistence.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoockerMT.Domain.Enums;
using StoockerMT.Domain.ValueObjects;
using StoockerMT.Persistence.Contexts;

namespace StoockerMT.Persistence.Repositories.TenantDb
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        private readonly TenantDbContext _context;

        public EmployeeRepository(TenantDbContext context):base(context)
        {
            _context = context;
        }


        public async Task<Employee?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(c => c.EmployeeCode == code, cancellationToken);
        }

        public async Task<Employee?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(c => c.Email != null && c.Email.Value == email.Value, cancellationToken);
        }

        public async Task<Employee?> GetWithLeavesAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Employees.Include(x=>x.Leaves)
                .FirstOrDefaultAsync(c=>c.Id==id, cancellationToken);
        }

        public async Task<Employee?> GetWithTimesheetsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Employees.Include(e => e.Timesheets)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Employee>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default)
        {
            return await _context.Employees.Where(e => e.DepartmentId == departmentId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Employee>> GetByPositionAsync(int positionId, CancellationToken cancellationToken = default)
        {
            return await _context.Employees.Where(e => e.PositionId == positionId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Employee>> GetByStatusAsync(EmploymentStatus status, CancellationToken cancellationToken = default)
        {
            return await _context.Employees
                .Where(e => e.Status == status)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Employee>> GetActiveEmployeesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Employees
                .Where(e => e.Status == EmploymentStatus.Active)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Employee>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
           return await _context.Employees
                .Where(e => e.FirstName.Contains(searchTerm) || e.LastName.Contains(searchTerm))
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _context.Employees
                .AnyAsync(e => e.EmployeeCode == code, cancellationToken);
        }

        public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
        {
           return await _context.Employees
                .AnyAsync(e => e.Email != null && e.Email.Value == email.Value, cancellationToken);
        }

        public async Task<bool> ExistsByNationalIdAsync(string nationalId, CancellationToken cancellationToken = default)
        {
            return await _context.Employees
                .AnyAsync(e => e.NationalId == nationalId, cancellationToken);
        }
    }
}
