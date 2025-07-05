using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Exceptions.Accounting
{
    public class InvalidAccountingPeriodException : DomainException
    {
        public DateTime AttemptedDate { get; }
        public DateTime PeriodStart { get; }
        public DateTime PeriodEnd { get; }

        public InvalidAccountingPeriodException(DateTime attemptedDate, DateTime periodStart, DateTime periodEnd)
            : base("INVALID_ACCOUNTING_PERIOD",
                $"Cannot post entry for date '{attemptedDate:yyyy-MM-dd}'. " +
                $"Period is closed or not within allowed range ({periodStart:yyyy-MM-dd} - {periodEnd:yyyy-MM-dd}).")
        {
            AttemptedDate = attemptedDate;
            PeriodStart = periodStart;
            PeriodEnd = periodEnd;
        }
    }
}
