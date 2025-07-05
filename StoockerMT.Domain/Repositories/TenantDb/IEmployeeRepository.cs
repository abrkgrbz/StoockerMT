using StoockerMT.Domain.Entities.TenantDb;
using StoockerMT.Domain.Enums;
using StoockerMT.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Repositories.TenantDb
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<Employee?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
        Task<Employee?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
        Task<Employee?> GetWithLeavesAsync(int id, CancellationToken cancellationToken = default);
        Task<Employee?> GetWithTimesheetsAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Employee>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Employee>> GetByPositionAsync(int positionId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Employee>> GetByStatusAsync(EmploymentStatus status, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Employee>> GetActiveEmployeesAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Employee>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
        Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
        Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNationalIdAsync(string nationalId, CancellationToken cancellationToken = default);
    }
}
