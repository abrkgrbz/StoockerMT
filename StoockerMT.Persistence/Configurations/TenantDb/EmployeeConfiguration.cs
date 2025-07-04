using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Domain.Entities.TenantDb;

namespace StoockerMT.Persistence.Configurations.TenantDb
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.EmployeeCode)
                .HasMaxLength(50);

            builder.HasIndex(e => e.EmployeeCode)
                .IsUnique()
                .HasDatabaseName("IX_Employees_EmployeeCode")
                .HasFilter("[EmployeeCode] IS NOT NULL");

            // Value Object: Email
            builder.OwnsOne(e => e.Email, email =>
            {
                email.Property(em => em.Value)
                    .HasColumnName("Email")
                    .IsRequired()
                    .HasMaxLength(200);

                email.HasIndex(em => em.Value)
                    .IsUnique()
                    .HasDatabaseName("IX_Employees_Email");
            });

            // Value Object: PhoneNumber (optional)
            builder.OwnsOne(e => e.PhoneNumber, phone =>
            {
                phone.Property(p => p.Value)
                    .HasColumnName("PhoneNumber")
                    .HasMaxLength(20);
            });

            // Value Object: Money for Salary
            builder.OwnsOne(e => e.Salary, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("Salary")
                    .HasColumnType("decimal(18,2)")
                    .HasDefaultValue(0);

                money.Property(m => m.Currency)
                    .HasColumnName("SalaryCurrency")
                    .HasMaxLength(3)
                    .HasDefaultValue("USD");
            });

            builder.Property(e => e.Status)
                .HasConversion<int>();

            // Value Object: Address (optional)
            builder.OwnsOne(e => e.Address, address =>
            {
                address.Property(a => a.Street).HasColumnName("Street").HasMaxLength(500);
                address.Property(a => a.City).HasColumnName("City").HasMaxLength(100);
                address.Property(a => a.State).HasColumnName("State").HasMaxLength(100);
                address.Property(a => a.Country).HasColumnName("Country").HasMaxLength(100);
                address.Property(a => a.ZipCode).HasColumnName("ZipCode").HasMaxLength(20);
            });

            builder.Property(e => e.NationalId)
                .HasMaxLength(50);

            builder.Property(e => e.CreatedBy)
                .HasMaxLength(100);

            builder.Property(e => e.UpdatedBy)
                .HasMaxLength(100);

            builder.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(e => e.Position)
                .WithMany(p => p.Employees)
                .HasForeignKey(e => e.PositionId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(e => e.Leaves)
                .WithOne(l => l.Employee)
                .HasForeignKey(l => l.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Timesheets)
                .WithOne(t => t.Employee)
                .HasForeignKey(t => t.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
