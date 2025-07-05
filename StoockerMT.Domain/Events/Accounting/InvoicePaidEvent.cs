using StoockerMT.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Events.Accounting
{
    public class InvoicePaidEvent : DomainEvent
    {
        public int InvoiceId { get; }
        public string InvoiceNumber { get; }
        public Money PaidAmount { get; }
        public string PaymentReference { get; }

        public InvoicePaidEvent(int invoiceId, string invoiceNumber, Money paidAmount, string paymentReference)
        {
            InvoiceId = invoiceId;
            InvoiceNumber = invoiceNumber;
            PaidAmount = paidAmount;
            PaymentReference = paymentReference;
        }
    }
}
