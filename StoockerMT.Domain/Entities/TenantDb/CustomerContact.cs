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
    public class CustomerContact : TenantBaseEntity
    {
        public int CustomerId { get; private set; }
        public string Name { get; private set; }
        public string? Title { get; private set; }
        public Email? Email { get; private set; }
        public PhoneNumber? PhoneNumber { get; private set; }
        public bool IsPrimary { get; private set; } = false;

        // Navigation Properties
        public virtual Customer Customer { get; set; }

        private CustomerContact() { }

        public CustomerContact(int customerId, string name, bool isPrimary = false)
        {
            CustomerId = customerId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            IsPrimary = isPrimary;
        }

        public void UpdateContactDetails(string name, string title, Email email, PhoneNumber phoneNumber)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Title = title;
            Email = email;
            PhoneNumber = phoneNumber;
            UpdateTimestamp();
        }

        public void SetAsPrimary()
        {
            IsPrimary = true;
            UpdateTimestamp();
        }

        public void UnsetAsPrimary()
        {
            IsPrimary = false;
            UpdateTimestamp();
        }
    }
}
