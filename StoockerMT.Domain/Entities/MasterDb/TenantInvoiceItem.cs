using StoockerMT.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Domain.ValueObjects;

namespace StoockerMT.Domain.Entities.MasterDb
{
    public class TenantInvoiceItem : BaseEntity
    {
        public int InvoiceId { get; private set; }
        public int? ModuleId { get; private set; }
        public string Description { get; private set; }
        public Quantity Quantity { get; private set; }
        public Money UnitPrice { get; private set; }
        public Money Total { get; private set; }

        // Navigation Properties
        public virtual TenantInvoice Invoice { get; set; }
        public virtual Module Module { get; set; }

        private TenantInvoiceItem() { }

        public TenantInvoiceItem(int invoiceId, string description, int quantity, Money unitPrice, int? moduleId = null)
        {
            InvoiceId = invoiceId;
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Quantity = new Quantity(quantity, "PCS");
            UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
            ModuleId = moduleId;
            CalculateTotal();
        }

        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(newQuantity));

            Quantity = new Quantity(newQuantity, Quantity.Unit);
            CalculateTotal();
        }

        public void UpdateUnitPrice(Money newUnitPrice)
        {
            UnitPrice = newUnitPrice ?? throw new ArgumentNullException(nameof(newUnitPrice));
            CalculateTotal();
        }

        private void CalculateTotal()
        {
            Total = UnitPrice.Multiply(Quantity.Value);
            UpdateTimestamp();
        }
    }
} 
