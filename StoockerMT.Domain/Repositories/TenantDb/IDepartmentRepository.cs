using StoockerMT.Domain.Entities.TenantDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Repositories.TenantDb
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        Task<Department?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<Department?> GetWithEmployeesAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Department>> GetActiveAsync(CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<int> GetEmployeeCountAsync(int departmentId, CancellationToken cancellationToken = default);
    }
}
