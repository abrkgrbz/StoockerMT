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
    public interface IAccountRepository : IRepository<Account>
    {
        Task<Account?> GetByCodeAsync(AccountCode code, CancellationToken cancellationToken = default);
        Task<Account?> GetWithSubAccountsAsync(int id, CancellationToken cancellationToken = default);
        Task<Account?> GetWithJournalLinesAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Account>> GetByTypeAsync(AccountType type, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Account>> GetActiveAccountsAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Account>> GetChildAccountsAsync(int parentAccountId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Account>> GetRootAccountsAsync(CancellationToken cancellationToken = default);
        Task<bool> ExistsByCodeAsync(AccountCode code, CancellationToken cancellationToken = default);
        Task<Money> GetTotalBalanceByTypeAsync(AccountType type, CancellationToken cancellationToken = default);
    }
}
