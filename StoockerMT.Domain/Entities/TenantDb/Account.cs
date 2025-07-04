using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Domain.Entities.TenantDb.Common;
using StoockerMT.Domain.Enums;
using StoockerMT.Domain.ValueObjects;

namespace StoockerMT.Domain.Entities.TenantDb
{
    public class Account : TenantBaseEntity
    {
        public AccountCode AccountCode { get; private set; }
        public string AccountName { get; private set; }
        public AccountType Type { get; private set; }
        public int? ParentAccountId { get; private set; }
        public Money Balance { get; private set; }
        public bool IsActive { get; private set; } = true;
        public string? Description { get; private set; }

        // Navigation Properties
        public virtual Account ParentAccount { get; set; }
        public virtual ICollection<Account> SubAccounts { get; set; } = new List<Account>();
        public virtual ICollection<JournalEntryLine> JournalEntryLines { get; set; } = new List<JournalEntryLine>();

        private Account() { }

        public Account(AccountCode accountCode, string accountName, AccountType type, string currency = "USD", int? parentAccountId = null)
        {
            AccountCode = accountCode ?? throw new ArgumentNullException(nameof(accountCode));
            AccountName = accountName ?? throw new ArgumentNullException(nameof(accountName));
            Type = type;
            ParentAccountId = parentAccountId;
            Balance = Money.Zero(currency);
        }

        public void UpdateBalance(Money amount)
        {
            if (amount.Currency != Balance.Currency)
                throw new ArgumentException("Currency mismatch");

            Balance = amount;
            UpdateTimestamp();
        }

        public void Debit(Money amount)
        {
            if (amount.Currency != Balance.Currency)
                throw new ArgumentException("Currency mismatch");

            // For Asset and Expense accounts, debit increases the balance
            // For Liability, Equity, and Revenue accounts, debit decreases the balance
            if (Type == AccountType.Asset || Type == AccountType.Expense)
            {
                Balance = Balance.Add(amount);
            }
            else
            {
                Balance = Balance.Subtract(amount);
            }

            UpdateTimestamp();
        }

        public void Credit(Money amount)
        {
            if (amount.Currency != Balance.Currency)
                throw new ArgumentException("Currency mismatch");

            // For Asset and Expense accounts, credit decreases the balance
            // For Liability, Equity, and Revenue accounts, credit increases the balance
            if (Type == AccountType.Asset || Type == AccountType.Expense)
            {
                Balance = Balance.Subtract(amount);
            }
            else
            {
                Balance = Balance.Add(amount);
            }

            UpdateTimestamp();
        }

        public void Activate()
        {
            IsActive = true;
            UpdateTimestamp();
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdateTimestamp();
        }

        public bool IsSubAccountOf(Account potentialParent)
        {
            if (potentialParent == null) return false;
            return AccountCode.IsSubAccountOf(potentialParent.AccountCode);
        }
    }
}
