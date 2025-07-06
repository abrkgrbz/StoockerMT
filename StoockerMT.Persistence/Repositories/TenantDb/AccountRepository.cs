using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoockerMT.Domain.Entities.TenantDb;
using StoockerMT.Domain.Enums;
using StoockerMT.Domain.Repositories.TenantDb;
using StoockerMT.Domain.ValueObjects;
using StoockerMT.Persistence.Contexts;
using StoockerMT.Persistence.Repositories.Common;

namespace StoockerMT.Persistence.Repositories.TenantDb
{
    public class AccountRepository:RepositoryBase<Account>, IAccountRepository
    {
        private readonly TenantDbContext _context;

        public AccountRepository(TenantDbContext context):base(context)
        {
            _context = context;
        }

        public async Task<Account?> GetByCodeAsync(AccountCode code, CancellationToken cancellationToken = default)
        {
            return await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountCode == code, cancellationToken);
        }

        public async Task<Account?> GetWithSubAccountsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Accounts
                .Include(a => a.SubAccounts)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }

        public async Task<Account?> GetWithJournalLinesAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Accounts
                .Include(a => a.JournalEntryLines)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Account>> GetByTypeAsync(AccountType type, CancellationToken cancellationToken = default)
        {
            return await _context.Accounts
                .Where(a => a.Type == type)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Account>> GetActiveAccountsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Accounts
                .Where(a => a.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Account>> GetChildAccountsAsync(int parentAccountId, CancellationToken cancellationToken = default)
        {
            return await _context.Accounts
                .Where(a => a.ParentAccountId == parentAccountId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Account>> GetRootAccountsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Accounts
                .Where(a => a.ParentAccountId == null)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsByCodeAsync(AccountCode code, CancellationToken cancellationToken = default)
        {
            return await _context.Accounts
                .AnyAsync(a => a.AccountCode == code, cancellationToken);
        }

        public async Task<Money> GetTotalBalanceByTypeAsync(AccountType type, CancellationToken cancellationToken = default)
        {
           var accounts = await _context.Accounts
                .Where(a => a.Type == type)
                .ToListAsync(cancellationToken);
            if (!accounts.Any())
            {
                return Money.Zero("USD");
            }
            var currency= accounts.First().Balance.Currency;
            var totalBalance = accounts.Sum(a => a.Balance.Amount);
            return new Money(totalBalance, currency);
        }
    }
}
