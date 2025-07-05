using StoockerMT.Domain.Entities.TenantDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Repositories.TenantDb
{
    public interface IPositionRepository : IRepository<Position>
    {
        Task<Position?> GetByTitleAsync(string title, CancellationToken cancellationToken = default);
        Task<Position?> GetWithEmployeesAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Position>> GetActiveAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Position>> GetBySalaryRangeAsync(decimal minSalary, decimal maxSalary, CancellationToken cancellationToken = default);
        Task<bool> ExistsByTitleAsync(string title, CancellationToken cancellationToken = default);
        Task<int> GetEmployeeCountAsync(int positionId, CancellationToken cancellationToken = default);
    }
}
