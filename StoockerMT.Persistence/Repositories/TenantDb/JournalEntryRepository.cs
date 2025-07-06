using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoockerMT.Domain.Entities.TenantDb;
using StoockerMT.Domain.Enums;
using StoockerMT.Domain.Repositories.TenantDb;
using StoockerMT.Persistence.Contexts;
using StoockerMT.Persistence.Repositories.Common;

namespace StoockerMT.Persistence.Repositories.TenantDb
{
    public class JournalEntryRepository:RepositoryBase<JournalEntry>, IJournalEntryRepository
    {
        private readonly TenantDbContext _context;

        public JournalEntryRepository(TenantDbContext context): base(context)
        {
            _context = context;
        }

        public async Task<JournalEntry?> GetByNumberAsync(string entryNumber, CancellationToken cancellationToken = default)
        {
            return await _context.JournalEntries
                .FirstOrDefaultAsync(je => je.EntryNumber == entryNumber, cancellationToken);
        }

        public async Task<JournalEntry?> GetWithLinesAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.JournalEntries
                .Include(je => je.Lines)
                .FirstOrDefaultAsync(je => je.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<JournalEntry>> GetByStatusAsync(JournalEntryStatus status, CancellationToken cancellationToken = default)
        {
            return await _context.JournalEntries
                .Where(je => je.Status == status)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<JournalEntry>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await _context.JournalEntries
                .Where(je => je.EntryDate >= startDate && je.EntryDate <= endDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<JournalEntry>> GetByAccountAsync(int accountId, CancellationToken cancellationToken = default)
        {
            return await _context.JournalEntries
                .Where(je => je.Lines.Any(line => line.AccountId == accountId))
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<JournalEntry>> GetUnbalancedEntriesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.JournalEntries
                .Where(je => je.TotalDebit.Amount != je.TotalCredit.Amount)
                .ToListAsync(cancellationToken);
        }

        public async Task<string> GenerateNextEntryNumberAsync(CancellationToken cancellationToken = default)
        {
            return await _context.JournalEntries
                .Select(je => je.EntryNumber)
                .OrderByDescending(en => en)
                .FirstOrDefaultAsync(cancellationToken) ?? "JE0000001"; // Default if no entries exist
        }

        public async Task<bool> ExistsByNumberAsync(string entryNumber, CancellationToken cancellationToken = default)
        {
            return await _context.JournalEntries
                .AnyAsync(je => je.EntryNumber == entryNumber, cancellationToken);
        }
    }
}
