using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Enums
{
    public enum InvoiceStatus
    {
        Draft = 1,
        Pending = 2,
        Paid = 3,
        Overdue = 4,
        Cancelled = 5
    }
}
