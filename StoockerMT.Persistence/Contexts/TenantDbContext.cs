using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using StoockerMT.Domain.Entities.Common;
using StoockerMT.Domain.Entities.TenantDb.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Domain.Entities.TenantDb;
using StoockerMT.Domain.Enums;

namespace StoockerMT.Persistence.Contexts
{
    public class TenantDbContext : DbContext
    {
        public TenantDbContext(DbContextOptions<TenantDbContext> options) : base(options) { }

        // Tenant Database Tables
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerContact> CustomerContacts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<InventoryMovement> InventoryMovements { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<EmployeeLeave> EmployeeLeaves { get; set; }
        public DbSet<EmployeeTimesheet> EmployeeTimesheets { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<JournalEntry> JournalEntries { get; set; }
        public DbSet<JournalEntryLine> JournalEntryLines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly(), t => t.Namespace.Contains("TenantDb"));

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(
                        Expression.Lambda(
                            Expression.Equal(
                                Expression.Property(
                                    Expression.Parameter(entityType.ClrType, "e"),
                                    nameof(BaseEntity.IsDeleted)
                                ),
                                Expression.Constant(false)
                            ),
                            Expression.Parameter(entityType.ClrType, "e")
                        )
                    );
                }
            }

            // Seed data
            SeedTenantData(modelBuilder);
        }

        private void SeedTenantData(ModelBuilder modelBuilder)
        {
            var seedDate = new DateTime(2025, 7, 5, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<ProductCategory>().HasData(
          new
          {
              Id = 1,
              CategoryName = "Electronics",
              Description = "Electronic products and accessories",
              ParentCategoryId = (int?)null,
              IsActive = true,
              CreatedAt = seedDate,
              UpdatedAt = (DateTime?)null,
              CreatedBy = (string)null,
              UpdatedBy = (string)null,
              IsDeleted = false
          },
          new
          {
              Id = 2,
              CategoryName = "Clothing",
              Description = "Clothing and fashion items",
              ParentCategoryId = (int?)null,
              IsActive = true,
              CreatedAt = seedDate,
              UpdatedAt = (DateTime?)null,
              CreatedBy = (string)null,
              UpdatedBy = (string)null,
              IsDeleted = false
          },
          new
          {
              Id = 3,
              CategoryName = "Books",
              Description = "Books and publications",
              ParentCategoryId = (int?)null,
              IsActive = true,
              CreatedAt = seedDate,
              UpdatedAt = (DateTime?)null,
              CreatedBy = (string)null,
              UpdatedBy = (string)null,
              IsDeleted = false
          },
          new
          {
              Id = 4,
              CategoryName = "Home & Garden",
              Description = "Home and garden products",
              ParentCategoryId = (int?)null,
              IsActive = true,
              CreatedAt = seedDate,
              UpdatedAt = (DateTime?)null,
              CreatedBy = (string)null,
              UpdatedBy = (string)null,
              IsDeleted = false
          }
      );

            // Seed Departments
            modelBuilder.Entity<Department>().HasData(
                new
                {
                    Id = 1,
                    DepartmentName = "Sales",
                    Description = "Sales department",
                    ManagerId = (int?)null,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    CreatedBy = (string)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 2,
                    DepartmentName = "Marketing",
                    Description = "Marketing department",
                    ManagerId = (int?)null,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    CreatedBy = (string)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 3,
                    DepartmentName = "IT",
                    Description = "Information Technology department",
                    ManagerId = (int?)null,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    CreatedBy = (string)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 4,
                    DepartmentName = "HR",
                    Description = "Human Resources department",
                    ManagerId = (int?)null,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    CreatedBy = (string)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 5,
                    DepartmentName = "Finance",
                    Description = "Finance department",
                    ManagerId = (int?)null,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    CreatedBy = (string)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false
                }
            );
             
            modelBuilder.Entity<Position>().HasData(
                new
                {
                    Id = 1,
                    Title = "Sales Manager",
                    Description = (string)null,
                    MinSalary = 50000m,
                    MaxSalary = 80000m,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    CreatedBy = (string)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 2,
                    Title = "Sales Representative",
                    Description = (string)null,
                    MinSalary = 30000m,
                    MaxSalary = 50000m,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    CreatedBy = (string)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 3,
                    Title = "Marketing Manager",
                    Description = (string)null,
                    MinSalary = 55000m,
                    MaxSalary = 85000m,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    CreatedBy = (string)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 4,
                    Title = "Software Developer",
                    Description = (string)null,
                    MinSalary = 60000m,
                    MaxSalary = 100000m,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    CreatedBy = (string)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 5,
                    Title = "HR Manager",
                    Description = (string)null,
                    MinSalary = 50000m,
                    MaxSalary = 75000m,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    CreatedBy = (string)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 6,
                    Title = "Accountant",
                    Description = (string)null,
                    MinSalary = 40000m,
                    MaxSalary = 65000m,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    CreatedBy = (string)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 7,
                    Title = "Finance Manager",
                    Description = (string)null,
                    MinSalary = 60000m,
                    MaxSalary = 90000m,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    CreatedBy = (string)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false
                }
            );
             
            modelBuilder.Entity<Account>().HasData(
                // Assets
                new
                {
                    Id = 1,
                    AccountName = "Cash",
                    Type = AccountType.Asset,
                    ParentAccountId = (int?)null,
                    IsActive = true,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    CreatedBy = (string)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 2,
                    AccountName = "Accounts Receivable",
                    Type = AccountType.Asset,
                    ParentAccountId = (int?)null,
                    IsActive = true,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    CreatedBy = (string)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 3,
                    AccountName = "Inventory",
                    Type = AccountType.Asset,
                    ParentAccountId = (int?)null,
                    IsActive = true,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    CreatedBy = (string)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 4,
                    AccountName = "Equipment",
                    Type = AccountType.Asset,
                    ParentAccountId = (int?)null,
                    IsActive = true,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    CreatedBy = (string)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 5,
                    AccountName = "Accumulated Depreciation",
                    Type = AccountType.Asset,
                    ParentAccountId = (int?)null,
                    IsActive = true,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    CreatedBy = (string)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false
                },

                // Liabilities
                new
                {
                    Id = 6,
                    AccountName = "Accounts Payable",
                    Type = AccountType.Liability,
                    ParentAccountId = (int?)null,
                    IsActive = true,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    CreatedBy = (string)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 7,
                    AccountName = "Accrued Expenses",
                    Type = AccountType.Liability,
                    ParentAccountId = (int?)null,
                    IsActive = true,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    CreatedBy = (string)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 8,
                    AccountName = "Notes Payable",
                    Type = AccountType.Liability,
                    ParentAccountId = (int?)null,
                    IsActive = true,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    CreatedBy = (string)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 9,
                    AccountName = "Unearned Revenue",
                    Type = AccountType.Liability,
                    ParentAccountId = (int?)null,
                    IsActive = true,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    CreatedBy = (string)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false
                },

                // Equity
                new
                {
                    Id = 10,
                    AccountName = "Owner's Equity",
                    Type = AccountType.Equity,
                    ParentAccountId = (int?)null,
                    IsActive = true,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    CreatedBy = (string)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 11,
                    AccountName = "Retained Earnings",
                    Type = AccountType.Equity,
                    ParentAccountId = (int?)null,
                    IsActive = true,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    CreatedBy = (string)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 12,
                    AccountName = "Dividends",
                    Type = AccountType.Equity,
                    ParentAccountId = (int?)null,
                    IsActive = true,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    CreatedBy = (string)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false
                },

                // Revenue
                new
                {
                    Id = 13,
                    AccountName = "Sales Revenue",
                    Type = AccountType.Revenue,
                    ParentAccountId = (int?)null,
                    IsActive = true,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    CreatedBy = (string)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 14,
                    AccountName = "Service Revenue",
                    Type = AccountType.Revenue,
                    ParentAccountId = (int?)null,
                    IsActive = true,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    CreatedBy = (string)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 15,
                    AccountName = "Interest Revenue",
                    Type = AccountType.Revenue,
                    ParentAccountId = (int?)null,
                    IsActive = true,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    CreatedBy = (string)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false
                },

                // Expenses
                new
                {
                    Id = 16,
                    AccountName = "Cost of Goods Sold",
                    Type = AccountType.Expense,
                    ParentAccountId = (int?)null,
                    IsActive = true,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    CreatedBy = (string)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 17,
                    AccountName = "Salary Expense",
                    Type = AccountType.Expense,
                    ParentAccountId = (int?)null,
                    IsActive = true,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    CreatedBy = (string)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 18,
                    AccountName = "Rent Expense",
                    Type = AccountType.Expense,
                    ParentAccountId = (int?)null,
                    IsActive = true,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    CreatedBy = (string)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 19,
                    AccountName = "Utilities Expense",
                    Type = AccountType.Expense,
                    ParentAccountId = (int?)null,
                    IsActive = true,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    CreatedBy = (string)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 20,
                    AccountName = "Depreciation Expense",
                    Type = AccountType.Expense,
                    ParentAccountId = (int?)null,
                    IsActive = true,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    CreatedBy = (string)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false
                }
            );
             
            modelBuilder.Entity<Account>()
                .OwnsOne(a => a.AccountCode)
                .HasData(
                    new { AccountId = 1, Value = "1000" },
                    new { AccountId = 2, Value = "1200" },
                    new { AccountId = 3, Value = "1300" },
                    new { AccountId = 4, Value = "1500" },
                    new { AccountId = 5, Value = "1510" },
                    new { AccountId = 6, Value = "2000" },
                    new { AccountId = 7, Value = "2100" },
                    new { AccountId = 8, Value = "2200" },
                    new { AccountId = 9, Value = "2300" },
                    new { AccountId = 10, Value = "3000" },
                    new { AccountId = 11, Value = "3100" },
                    new { AccountId = 12, Value = "3200" },
                    new { AccountId = 13, Value = "4000" },
                    new { AccountId = 14, Value = "4100" },
                    new { AccountId = 15, Value = "4200" },
                    new { AccountId = 16, Value = "5000" },
                    new { AccountId = 17, Value = "5100" },
                    new { AccountId = 18, Value = "5200" },
                    new { AccountId = 19, Value = "5300" },
                    new { AccountId = 20, Value = "5400" }
                );
             
            modelBuilder.Entity<Account>()
                .OwnsOne(a => a.Balance)
                .HasData(
                    new { AccountId = 1, Amount = 0m, Currency = "USD" },
                    new { AccountId = 2, Amount = 0m, Currency = "USD" },
                    new { AccountId = 3, Amount = 0m, Currency = "USD" },
                    new { AccountId = 4, Amount = 0m, Currency = "USD" },
                    new { AccountId = 5, Amount = 0m, Currency = "USD" },
                    new { AccountId = 6, Amount = 0m, Currency = "USD" },
                    new { AccountId = 7, Amount = 0m, Currency = "USD" },
                    new { AccountId = 8, Amount = 0m, Currency = "USD" },
                    new { AccountId = 9, Amount = 0m, Currency = "USD" },
                    new { AccountId = 10, Amount = 0m, Currency = "USD" },
                    new { AccountId = 11, Amount = 0m, Currency = "USD" },
                    new { AccountId = 12, Amount = 0m, Currency = "USD" },
                    new { AccountId = 13, Amount = 0m, Currency = "USD" },
                    new { AccountId = 14, Amount = 0m, Currency = "USD" },
                    new { AccountId = 15, Amount = 0m, Currency = "USD" },
                    new { AccountId = 16, Amount = 0m, Currency = "USD" },
                    new { AccountId = 17, Amount = 0m, Currency = "USD" },
                    new { AccountId = 18, Amount = 0m, Currency = "USD" },
                    new { AccountId = 19, Amount = 0m, Currency = "USD" },
                    new { AccountId = 20, Amount = 0m, Currency = "USD" }
                );
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateAuditFields()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is TenantBaseEntity &&
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (TenantBaseEntity)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.SetCreatedAt(DateTime.UtcNow);
                    // entity.CreatedBy should be set by the service
                }
                else if (entry.State == EntityState.Modified)
                {
                    entity.UpdateTimestamp();
                    // entity.UpdatedBy should be set by the service
                }
            }
        }
    }
}
