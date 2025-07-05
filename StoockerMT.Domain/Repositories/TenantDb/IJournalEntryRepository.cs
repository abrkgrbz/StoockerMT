using StoockerMT.Domain.Entities.TenantDb;
using StoockerMT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Repositories.TenantDb
{
    public interface IJournalEntryRepository : IRepository<JournalEntry>
    {
        Task<JournalEntry?> GetByNumberAsync(string entryNumber, CancellationToken cancellationToken = default);
        Task<JournalEntry?> GetWithLinesAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<JournalEntry>> GetByStatusAsync(JournalEntryStatus status, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<JournalEntry>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<JournalEntry>> GetByAccountAsync(int accountId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<JournalEntry>> GetUnbalancedEntriesAsync(CancellationToken cancellationToken = default);
        Task<string> GenerateNextEntryNumberAsync(CancellationToken cancellationToken = default);
        Task<bool> ExistsByNumberAsync(string entryNumber, CancellationToken cancellationToken = default);
    }
}
