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
    public class JournalEntry : TenantBaseEntity
    {
        public string EntryNumber { get; private set; }
        public DateTime EntryDate { get; private set; }
        public string? Description { get; private set; }
        public string? Reference { get; private set; }
        public Money TotalDebit { get; private set; }
        public Money TotalCredit { get; private set; }
        public JournalEntryStatus Status { get; private set; } = JournalEntryStatus.Draft;

        // Navigation Properties
        public virtual ICollection<JournalEntryLine> Lines { get; set; } = new List<JournalEntryLine>();

        private JournalEntry() { }

        public JournalEntry(string entryNumber, DateTime entryDate, string currency = "USD", string? description = null)
        {
            EntryNumber = entryNumber ?? throw new ArgumentNullException(nameof(entryNumber));
            EntryDate = entryDate;
            Description = description;
            TotalDebit = Money.Zero(currency);
            TotalCredit = Money.Zero(currency);
        }

        public void AddLine(Account account, Money debitAmount, Money creditAmount, string? description = null)
        {
            if (debitAmount.Currency != TotalDebit.Currency || creditAmount.Currency != TotalCredit.Currency)
                throw new ArgumentException("Currency mismatch");

            if (debitAmount.Amount > 0 && creditAmount.Amount > 0)
                throw new ArgumentException("A journal line cannot have both debit and credit amounts");

            if (debitAmount.Amount == 0 && creditAmount.Amount == 0)
                throw new ArgumentException("A journal line must have either a debit or credit amount");

            var line = new JournalEntryLine(Id, account.Id, debitAmount, creditAmount, description);
            Lines.Add(line);
            CalculateTotals();
        }

        private void CalculateTotals()
        {
            if (!Lines.Any())
            {
                TotalDebit = Money.Zero(TotalDebit.Currency);
                TotalCredit = Money.Zero(TotalCredit.Currency);
                return;
            }

            TotalDebit = Lines.Aggregate(
                Money.Zero(TotalDebit.Currency),
                (sum, line) => sum.Add(line.DebitAmount)
            );

            TotalCredit = Lines.Aggregate(
                Money.Zero(TotalCredit.Currency),
                (sum, line) => sum.Add(line.CreditAmount)
            );

            UpdateTimestamp();
        }

        public bool IsBalanced()
        {
            return TotalDebit.Amount == TotalCredit.Amount;
        }

        public void Post()
        {
            if (Status != JournalEntryStatus.Draft)
                throw new InvalidOperationException("Only draft entries can be posted");

            if (!IsBalanced())
                throw new InvalidOperationException("Cannot post an unbalanced journal entry");

            if (!Lines.Any())
                throw new InvalidOperationException("Cannot post an empty journal entry");

            Status = JournalEntryStatus.Posted;
            UpdateTimestamp();
        }

        public void Reverse(string reverseEntryNumber, string reason)
        {
            if (Status != JournalEntryStatus.Posted)
                throw new InvalidOperationException("Only posted entries can be reversed");

            Status = JournalEntryStatus.Reversed;
            Description = $"{Description} - REVERSED: {reason}";
            Reference = $"Reversed by {reverseEntryNumber}";
            UpdateTimestamp();
        }
    }
}
