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
    public class Customer : TenantBaseEntity
    {
        public string CustomerName { get; private set; }
        public string? CustomerCode { get; private set; }
        public string? ContactPerson { get; private set; }
        public Email? Email { get; private set; }
        public PhoneNumber? PhoneNumber { get; private set; }
        public Address? Address { get; private set; }
        public CustomerType Type { get; private set; } = CustomerType.Individual;
        public CustomerStatus Status { get; private set; } = CustomerStatus.Active;
        public Money CreditLimit { get; private set; }
        public string? Notes { get; private set; }

        // Navigation Properties
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<CustomerContact> Contacts { get; set; } = new List<CustomerContact>();

        private Customer() { }

        public Customer(string customerName, string? customerCode = null, string currency = "USD")
        {
            CustomerName = customerName ?? throw new ArgumentNullException(nameof(customerName));
            CustomerCode = customerCode;
            CreditLimit = Money.Zero(currency);
        }

        public void UpdateContactInfo(Email email, PhoneNumber phoneNumber, Address address)
        {
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
            UpdateTimestamp();
        }

        public void SetCreditLimit(Money creditLimit)
        {
            CreditLimit = creditLimit ?? throw new ArgumentNullException(nameof(creditLimit));
            UpdateTimestamp();
        }

        public void IncreaseCreditLimit(Money amount)
        {
            if (amount.Currency != CreditLimit.Currency)
                throw new ArgumentException("Currency mismatch");

            CreditLimit = CreditLimit.Add(amount);
            UpdateTimestamp();
        }

        public void DecreaseCreditLimit(Money amount)
        {
            if (amount.Currency != CreditLimit.Currency)
                throw new ArgumentException("Currency mismatch");

            var newLimit = CreditLimit.Subtract(amount);
            if (newLimit.Amount < 0)
                throw new InvalidOperationException("Credit limit cannot be negative");

            CreditLimit = newLimit;
            UpdateTimestamp();
        }

        public void ChangeStatus(CustomerStatus status, string notes = null)
        {
            Status = status;
            if (!string.IsNullOrWhiteSpace(notes))
                Notes = $"{Notes}\n{DateTime.UtcNow:yyyy-MM-dd}: {notes}";
            UpdateTimestamp();
        }

        public void ChangeType(CustomerType type)
        {
            Type = type;
            UpdateTimestamp();
        }
    }
} 
