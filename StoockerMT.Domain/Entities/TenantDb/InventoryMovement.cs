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
    public class InventoryMovement : TenantBaseEntity
    {
        public int ProductId { get; private set; }
        public MovementType Type { get; private set; }
        public Quantity Quantity { get; private set; }
        public Money UnitCost { get; private set; }
        public string? Reference { get; private set; }
        public string? Notes { get; private set; }
        public DateTime MovementDate { get; private set; }

        // Navigation Properties
        public virtual Product Product { get; set; }

        private InventoryMovement() { }

        public InventoryMovement(int productId, MovementType type, Quantity quantity, Money unitCost, string? reference = null)
        {
            ProductId = productId;
            Type = type;
            Quantity = quantity ?? throw new ArgumentNullException(nameof(quantity));
            UnitCost = unitCost ?? throw new ArgumentNullException(nameof(unitCost));

            if (quantity.Value <= 0)
                throw new ArgumentException("Movement quantity must be greater than zero", nameof(quantity));

            Reference = reference;
            MovementDate = DateTime.UtcNow;
        }

        public Money GetTotalValue()
        {
            return UnitCost.Multiply(Quantity.Value);
        }

        public void AddNotes(string notes)
        {
            Notes = notes;
            UpdateTimestamp();
        }
    }

}
