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
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
        Task<Customer?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
        Task<Customer?> GetWithOrdersAsync(int id, CancellationToken cancellationToken = default);
        Task<Customer?> GetWithContactsAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Customer>> GetByTypeAsync(CustomerType type, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Customer>> GetByStatusAsync(CustomerStatus status, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Customer>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
        Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
        Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default);
        Task<Money> GetTotalOutstandingBalanceAsync(int customerId, CancellationToken cancellationToken = default);
    }
}
