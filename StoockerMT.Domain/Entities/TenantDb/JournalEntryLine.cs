using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Domain.Entities.TenantDb.Common;
using StoockerMT.Domain.ValueObjects;

namespace StoockerMT.Domain.Entities.TenantDb
{
    public class JournalEntryLine : TenantBaseEntity
    {
        public int JournalEntryId { get; private set; }
        public int AccountId { get; private set; }
        public string? Description { get; private set; }
        public Money DebitAmount { get; private set; }
        public Money CreditAmount { get; private set; }

        // Navigation Properties
        public virtual JournalEntry JournalEntry { get; set; }
        public virtual Account Account { get; set; }

        private JournalEntryLine() { }

        public JournalEntryLine(int journalEntryId, int accountId, Money debitAmount, Money creditAmount, string? description = null)
        {
            JournalEntryId = journalEntryId;
            AccountId = accountId;
            DebitAmount = debitAmount ?? throw new ArgumentNullException(nameof(debitAmount));
            CreditAmount = creditAmount ?? throw new ArgumentNullException(nameof(creditAmount));
            Description = description;

            if (debitAmount.Currency != creditAmount.Currency)
                throw new ArgumentException("Debit and credit amounts must have the same currency");

            if (debitAmount.Amount > 0 && creditAmount.Amount > 0)
                throw new ArgumentException("A journal line cannot have both debit and credit amounts");

            if (debitAmount.Amount == 0 && creditAmount.Amount == 0)
                throw new ArgumentException("A journal line must have either a debit or credit amount");
        }

        public Money GetAmount()
        {
            return DebitAmount.Amount > 0 ? DebitAmount : CreditAmount;
        }

        public bool IsDebit()
        {
            return DebitAmount.Amount > 0;
        }

        public bool IsCredit()
        {
            return CreditAmount.Amount > 0;
        }
    }
}
