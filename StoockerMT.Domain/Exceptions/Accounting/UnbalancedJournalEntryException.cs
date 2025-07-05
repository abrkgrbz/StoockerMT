using StoockerMT.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Exceptions.Accounting
{
    public class UnbalancedJournalEntryException : DomainException
    {
        public Money TotalDebit { get; }
        public Money TotalCredit { get; }

        public UnbalancedJournalEntryException(Money totalDebit, Money totalCredit)
            : base("UNBALANCED_JOURNAL_ENTRY",
                $"Journal entry is not balanced. Debit: {totalDebit}, Credit: {totalCredit}")
        {
            TotalDebit = totalDebit;
            TotalCredit = totalCredit;
        }
    }
}
