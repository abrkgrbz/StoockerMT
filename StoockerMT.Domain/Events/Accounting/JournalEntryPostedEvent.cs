using StoockerMT.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Events.Accounting
{
    public class JournalEntryPostedEvent : DomainEvent
    {
        public int JournalEntryId { get; }
        public string EntryNumber { get; }
        public Money TotalAmount { get; }
        public DateTime PostedDate { get; }

        public JournalEntryPostedEvent(int journalEntryId, string entryNumber, Money totalAmount, DateTime postedDate)
        {
            JournalEntryId = journalEntryId;
            EntryNumber = entryNumber;
            TotalAmount = totalAmount;
            PostedDate = postedDate;
        }
    }
}
