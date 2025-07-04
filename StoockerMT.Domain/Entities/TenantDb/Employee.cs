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
    public class Employee : TenantBaseEntity
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string? EmployeeCode { get; private set; }
        public Email Email { get; private set; }
        public PhoneNumber? PhoneNumber { get; private set; }
        public DateTime HireDate { get; private set; }
        public DateTime? TerminationDate { get; private set; }
        public int? DepartmentId { get; private set; }
        public int? PositionId { get; private set; }
        public Money Salary { get; private set; }
        public EmploymentStatus Status { get; private set; } = EmploymentStatus.Active;
        public Address? Address { get; private set; }
        public DateTime? DateOfBirth { get; private set; }
        public string? NationalId { get; private set; }

        // Navigation Properties
        public virtual Department Department { get; set; }
        public virtual Position Position { get; set; }
        public virtual ICollection<EmployeeLeave> Leaves { get; set; } = new List<EmployeeLeave>();
        public virtual ICollection<EmployeeTimesheet> Timesheets { get; set; } = new List<EmployeeTimesheet>();

        private Employee() { }

        public Employee(string firstName, string lastName, Email email, DateTime hireDate, Money salary)
        {
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            HireDate = hireDate;
            Salary = salary ?? throw new ArgumentNullException(nameof(salary));
        }

        public string FullName => $"{FirstName} {LastName}";

        public void UpdatePersonalInfo(string firstName, string lastName, Email email, PhoneNumber phoneNumber, Address address)
        {
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            PhoneNumber = phoneNumber;
            Address = address;
            UpdateTimestamp();
        }

        public void UpdateSalary(Money newSalary)
        {
            Salary = newSalary ?? throw new ArgumentNullException(nameof(newSalary));
            UpdateTimestamp();
        }

        public void GiveRaise(Percentage raisePercentage)
        {
            var raiseAmount = raisePercentage.ApplyTo(Salary);
            Salary = Salary.Add(raiseAmount);
            UpdateTimestamp();
        }

        public void AssignToDepartment(int departmentId)
        {
            DepartmentId = departmentId;
            UpdateTimestamp();
        }

        public void AssignToPosition(int positionId)
        {
            PositionId = positionId;
            UpdateTimestamp();
        }

        public void Terminate(DateTime terminationDate)
        {
            if (terminationDate < HireDate)
                throw new ArgumentException("Termination date cannot be before hire date");

            TerminationDate = terminationDate;
            Status = EmploymentStatus.Terminated;
            UpdateTimestamp();
        }

        public void SetOnLeave()
        {
            Status = EmploymentStatus.OnLeave;
            UpdateTimestamp();
        }

        public void ReturnFromLeave()
        {
            Status = EmploymentStatus.Active;
            UpdateTimestamp();
        }

        public int GetYearsOfService()
        {
            var endDate = TerminationDate ?? DateTime.UtcNow;
            return (int)((endDate - HireDate).TotalDays / 365.25);
        }
    }
}
