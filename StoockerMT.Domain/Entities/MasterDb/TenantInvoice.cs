using StoockerMT.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Domain.Enums;
using StoockerMT.Domain.ValueObjects;

namespace StoockerMT.Domain.Entities.MasterDb
{
    public class TenantInvoice : BaseEntity
    {
        public int TenantId { get; private set; }
        public InvoiceNumber InvoiceNumber { get; private set; }
        public DateRange BillingPeriod { get; private set; }
        public DateTime DueDate { get; private set; }
        public Money SubTotal { get; private set; }
        public Money TaxAmount { get; private set; }
        public Money Total { get; private set; }
        public InvoiceStatus Status { get; private set; } = InvoiceStatus.Pending;
        public DateTime? PaidAt { get; private set; }
        public string? Notes { get; private set; }
        public string? PaymentReference { get; private set; }

        // Navigation Properties
        public virtual Tenant Tenant { get; set; }
        public virtual ICollection<TenantInvoiceItem> Items { get; set; } = new List<TenantInvoiceItem>();

        private TenantInvoice() { }

        public TenantInvoice(int tenantId, InvoiceNumber invoiceNumber, DateRange billingPeriod, DateTime dueDate)
        {
            TenantId = tenantId;
            InvoiceNumber = invoiceNumber ?? throw new ArgumentNullException(nameof(invoiceNumber));
            BillingPeriod = billingPeriod ?? throw new ArgumentNullException(nameof(billingPeriod));
            DueDate = dueDate;
            CreatedAt = DateTime.UtcNow;
        }

        public void AddItem(string description, int quantity, Money unitPrice, int? moduleId = null)
        {
            var item = new TenantInvoiceItem(Id, description, quantity, unitPrice, moduleId);
            Items.Add(item);
            CalculateTotals();
        }

        public void CalculateTotals()
        {
            if (!Items.Any())
            {
                SubTotal = Money.Zero();
                TaxAmount = Money.Zero();
                Total = Money.Zero();
                return;
            }

            var currency = Items.First().Total.Currency;
            SubTotal = Items.Aggregate(
                Money.Zero(currency),
                (sum, item) => sum.Add(item.Total)
            );

            // Assuming 18% tax rate
            TaxAmount = SubTotal.Multiply(0.18m);
            Total = SubTotal.Add(TaxAmount);

            UpdateTimestamp();
        }

        public void MarkAsPaid(string paymentReference = null)
        {
            if (Status == InvoiceStatus.Paid)
                throw new InvalidOperationException("Invoice is already paid");

            Status = InvoiceStatus.Paid;
            PaidAt = DateTime.UtcNow;
            PaymentReference = paymentReference;
            UpdateTimestamp();
        }

        public void MarkAsOverdue()
        {
            if (Status == InvoiceStatus.Paid || Status == InvoiceStatus.Cancelled)
                throw new InvalidOperationException($"Cannot mark {Status} invoice as overdue");

            if (DateTime.UtcNow > DueDate)
            {
                Status = InvoiceStatus.Overdue;
                UpdateTimestamp();
            }
        }

        public void Cancel(string reason)
        {
            if (Status == InvoiceStatus.Paid)
                throw new InvalidOperationException("Cannot cancel a paid invoice");

            Status = InvoiceStatus.Cancelled;
            Notes = $"Cancelled: {reason}";
            UpdateTimestamp();
        }

        public void AddNotes(string notes)
        {
            Notes = notes;
            UpdateTimestamp();
        }
    }
}
 
