using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using StoockerMT.Domain.Entities.Common;
using StoockerMT.Domain.Entities.MasterDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Module = StoockerMT.Domain.Entities.MasterDb.Module;
using StoockerMT.Domain.Enums;
using StoockerMT.Domain.ValueObjects;
using StoockerMT.Domain.Entities.TenantDb;

namespace StoockerMT.Persistence.Contexts
{
    public class MasterDbContext : DbContext
    {
        public MasterDbContext(DbContextOptions<MasterDbContext> options) : base(options)
        {
        }

        // Master Database Tables
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<ModulePermission> ModulePermissions { get; set; }
        public DbSet<ModuleFeature> ModuleFeatures { get; set; }
        public DbSet<TenantModuleSubscription> TenantModuleSubscriptions { get; set; }
        public DbSet<TenantUser> TenantUsers { get; set; }
        public DbSet<TenantUserPermission> TenantUserPermissions { get; set; }
        public DbSet<TenantModuleUsage> TenantModuleUsages { get; set; }
        public DbSet<TenantInvoice> TenantInvoices { get; set; }
        public DbSet<TenantInvoiceItem> TenantInvoiceItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
             
            modelBuilder.Ignore<DatabaseInfo>();
            modelBuilder.Ignore<TenantSettings>();
            modelBuilder.Ignore<TenantCode>();
             
            ConfigureTenant(modelBuilder);
             
            modelBuilder.ApplyConfigurationsFromAssembly(
                Assembly.GetExecutingAssembly(),
                t => t.Namespace != null && t.Namespace.Contains("MasterDb") &&
                     t.Name != "TenantConfiguration"  
            );
             
            ApplyGlobalQueryFilters(modelBuilder);


            // Seed data
            SeedMasterData(modelBuilder);
        }

        private void SeedMasterData(ModelBuilder modelBuilder)
        {
            var seedDate = new DateTime(2025, 7, 5, 0, 0, 0, DateTimeKind.Utc); // Statik tarih
 
            modelBuilder.Entity<Module>().HasData(
                new
                {
                    Id = 1,
                    Name = "Customer Relationship Management",
                    Code = "CRM",
                    Category = ModuleCategory.CRM,
                    CreatedBy = "System",
                    Description = "Complete CRM solution for managing customers, leads, and sales",
                    Version = "1.0.0",
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false,
                    Configuration = (string)null,
                    Dependencies = (string)null
                },
                new
                {
                    Id = 2,
                    Name = "Inventory Management",
                    Code = "INV",
                    Category = ModuleCategory.Inventory,
                    CreatedBy = "System",
                    Description = "Inventory and warehouse management system",
                    Version = "1.0.0",
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false,
                    Configuration = (string)null,
                    Dependencies = (string)null
                },
                new
                {
                    Id = 3,
                    Name = "Accounting",
                    Code = "ACC",
                    Category = ModuleCategory.Accounting,
                    CreatedBy = "System",
                    Description = "Financial accounting and reporting system",
                    Version = "1.0.0",
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false,
                    Configuration = (string)null,
                    Dependencies = (string)null
                },
                new
                {
                    Id = 4,
                    Name = "Human Resources",
                    Code = "HR",
                    Category = ModuleCategory.HR,
                    CreatedBy = "System",
                    Description = "Employee management and HR processes",
                    Version = "1.0.0",
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    UpdatedBy = (string)null,
                    IsDeleted = false,
                    Configuration = (string)null,
                    Dependencies = (string)null
                }
            );
             
            modelBuilder.Entity<Module>()
                .OwnsOne(m => m.MonthlyPrice)
                .HasData(
                    new { ModuleId = 1, Amount = 99.99m, Currency = "USD" },
                    new { ModuleId = 2, Amount = 149.99m, Currency = "USD" },
                    new { ModuleId = 3, Amount = 199.99m, Currency = "USD" },
                    new { ModuleId = 4, Amount = 129.99m, Currency = "USD" }
                );
             
            modelBuilder.Entity<Module>()
                .OwnsOne(m => m.YearlyPrice)
                .HasData(
                    new { ModuleId = 1, Amount = 999.99m, Currency = "USD" },
                    new { ModuleId = 2, Amount = 1499.99m, Currency = "USD" },
                    new { ModuleId = 3, Amount = 1999.99m, Currency = "USD" },
                    new { ModuleId = 4, Amount = 1299.99m, Currency = "USD" }
                );

            // Seed Module Permissions
            modelBuilder.Entity<ModulePermission>().HasData(
                // CRM Permissions
                new
                {
                    Id = 1,
                    ModuleId = 1,
                    Name = "View Customers",
                    Code = "CRM_VIEW_CUSTOMERS",
                    Type = PermissionType.Read,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 2,
                    ModuleId = 1,
                    Name = "Manage Customers",
                    Code = "CRM_MANAGE_CUSTOMERS",
                    Type = PermissionType.Write,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 3,
                    ModuleId = 1,
                    Name = "Delete Customers",
                    Code = "CRM_DELETE_CUSTOMERS",
                    Type = PermissionType.Delete,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 4,
                    ModuleId = 1,
                    Name = "CRM Admin",
                    Code = "CRM_ADMIN",
                    Type = PermissionType.Admin,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    IsDeleted = false
                },

                // Inventory Permissions
                new
                {
                    Id = 5,
                    ModuleId = 2,
                    Name = "View Products",
                    Code = "INV_VIEW_PRODUCTS",
                    Type = PermissionType.Read,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 6,
                    ModuleId = 2,
                    Name = "Manage Products",
                    Code = "INV_MANAGE_PRODUCTS",
                    Type = PermissionType.Write,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 7,
                    ModuleId = 2,
                    Name = "Manage Inventory",
                    Code = "INV_MANAGE_INVENTORY",
                    Type = PermissionType.Write,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 8,
                    ModuleId = 2,
                    Name = "Inventory Admin",
                    Code = "INV_ADMIN",
                    Type = PermissionType.Admin,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    IsDeleted = false
                },

                // Accounting Permissions
                new
                {
                    Id = 9,
                    ModuleId = 3,
                    Name = "View Accounts",
                    Code = "ACC_VIEW_ACCOUNTS",
                    Type = PermissionType.Read,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 10,
                    ModuleId = 3,
                    Name = "Manage Accounts",
                    Code = "ACC_MANAGE_ACCOUNTS",
                    Type = PermissionType.Write,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 11,
                    ModuleId = 3,
                    Name = "View Reports",
                    Code = "ACC_VIEW_REPORTS",
                    Type = PermissionType.Read,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 12,
                    ModuleId = 3,
                    Name = "Accounting Admin",
                    Code = "ACC_ADMIN",
                    Type = PermissionType.Admin,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    IsDeleted = false
                },

                // HR Permissions
                new
                {
                    Id = 13,
                    ModuleId = 4,
                    Name = "View Employees",
                    Code = "HR_VIEW_EMPLOYEES",
                    Type = PermissionType.Read,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 14,
                    ModuleId = 4,
                    Name = "Manage Employees",
                    Code = "HR_MANAGE_EMPLOYEES",
                    Type = PermissionType.Write,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 15,
                    ModuleId = 4,
                    Name = "Manage Payroll",
                    Code = "HR_MANAGE_PAYROLL",
                    Type = PermissionType.Write,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 16,
                    ModuleId = 4,
                    Name = "HR Admin",
                    Code = "HR_ADMIN",
                    Type = PermissionType.Admin,
                    Description = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    IsDeleted = false
                }
            );

            // Seed Module Features
            modelBuilder.Entity<ModuleFeature>().HasData(
                // CRM Features
                new
                {
                    Id = 1,
                    ModuleId = 1,
                    Name = "Lead Management",
                    Code = "CRM_LEADS",
                    Description = "Manage leads and opportunities",
                    IsEnabled = true,
                    Configuration = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 2,
                    ModuleId = 1,
                    Name = "Contact Management",
                    Code = "CRM_CONTACTS",
                    Description = "Manage customer contacts",
                    IsEnabled = true,
                    Configuration = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 3,
                    ModuleId = 1,
                    Name = "Sales Pipeline",
                    Code = "CRM_PIPELINE",
                    Description = "Visual sales pipeline management",
                    IsEnabled = true,
                    Configuration = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    IsDeleted = false
                },

                // Inventory Features
                new
                {
                    Id = 4,
                    ModuleId = 2,
                    Name = "Stock Tracking",
                    Code = "INV_STOCK",
                    Description = "Real-time stock tracking",
                    IsEnabled = true,
                    Configuration = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 5,
                    ModuleId = 2,
                    Name = "Barcode Scanning",
                    Code = "INV_BARCODE",
                    Description = "Barcode scanning support",
                    IsEnabled = true,
                    Configuration = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 6,
                    ModuleId = 2,
                    Name = "Reorder Management",
                    Code = "INV_REORDER",
                    Description = "Automatic reorder points",
                    IsEnabled = true,
                    Configuration = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    IsDeleted = false
                },

                // Accounting Features
                new
                {
                    Id = 7,
                    ModuleId = 3,
                    Name = "General Ledger",
                    Code = "ACC_GL",
                    Description = "General ledger management",
                    IsEnabled = true,
                    Configuration = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 8,
                    ModuleId = 3,
                    Name = "Financial Reports",
                    Code = "ACC_REPORTS",
                    Description = "Financial reporting suite",
                    IsEnabled = true,
                    Configuration = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 9,
                    ModuleId = 3,
                    Name = "Tax Management",
                    Code = "ACC_TAX",
                    Description = "Tax calculation and reporting",
                    IsEnabled = true,
                    Configuration = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    IsDeleted = false
                },

                // HR Features
                new
                {
                    Id = 10,
                    ModuleId = 4,
                    Name = "Leave Management",
                    Code = "HR_LEAVE",
                    Description = "Employee leave management",
                    IsEnabled = true,
                    Configuration = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 11,
                    ModuleId = 4,
                    Name = "Timesheet",
                    Code = "HR_TIMESHEET",
                    Description = "Employee timesheet tracking",
                    IsEnabled = true,
                    Configuration = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    IsDeleted = false
                },
                new
                {
                    Id = 12,
                    ModuleId = 4,
                    Name = "Payroll",
                    Code = "HR_PAYROLL",
                    Description = "Payroll processing",
                    IsEnabled = true,
                    Configuration = (string)null,
                    CreatedAt = seedDate,
                    UpdatedAt = (DateTime?)null,
                    IsDeleted = false
                }
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
                .Where(e => e.Entity is IAuditableEntity &&
                            (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (IAuditableEntity)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                    // entity.CreatedBy should be set by the service
                }
                else if (entry.State == EntityState.Modified)
                {
                    entity.UpdatedAt = DateTime.UtcNow;
                    // entity.UpdatedBy should be set by the service
                }
            }
        }

        private void ConfigureTenant(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tenant>(entity =>
            {
                entity.ToTable("Tenants");
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(t => t.Description)
                    .HasMaxLength(1000);

                entity.Property(t => t.Status)
                    .HasConversion<int>()
                    .IsRequired();

                entity.Property(t => t.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(t => t.UpdatedBy)
                    .HasMaxLength(100);

                entity.Property(t => t.CreatedAt)
                    .IsRequired();

                entity.Property(t => t.UpdatedAt);

                entity.Property(t => t.MaxUsers)
                    .HasDefaultValue(10)
                    .IsRequired();

                entity.Property(t => t.MaxStorageBytes)
                    .HasDefaultValue(1073741824L)
                    .IsRequired();

                entity.Property(t => t.MaxModules)
                    .HasDefaultValue(5)
                    .IsRequired();

                entity.Property(t => t.ActivatedDate);
                entity.Property(t => t.DeactivatedDate);
                entity.Property(t => t.DeactivationReason).HasMaxLength(500);

                // Configure TenantCode as owned
                entity.OwnsOne(t => t.Code, code =>
                {
                    code.WithOwner();

                    code.Property(c => c.Value)
                        .HasColumnName("Code")
                        .IsRequired()
                        .HasMaxLength(50);

                    code.HasIndex(c => c.Value)
                        .IsUnique()
                        .HasDatabaseName("IX_Tenants_Code");
                });

                // Configure TenantSettings as owned
                entity.OwnsOne(t => t.Settings, settings =>
                {
                    settings.WithOwner();

                    settings.Property(s => s.TimeZone).HasColumnName("Settings_TimeZone").HasMaxLength(50);
                    settings.Property(s => s.DateFormat).HasColumnName("Settings_DateFormat").HasMaxLength(20);
                    settings.Property(s => s.TimeFormat).HasColumnName("Settings_TimeFormat").HasMaxLength(20);
                    settings.Property(s => s.Language).HasColumnName("Settings_Language").HasMaxLength(10);
                    settings.Property(s => s.Currency).HasColumnName("Settings_Currency").HasMaxLength(3);
                    settings.Property(s => s.FiscalYearStartMonth).HasColumnName("Settings_FiscalYearStartMonth");
                    settings.Property(s => s.FirstDayOfWeek).HasColumnName("Settings_FirstDayOfWeek");
                    settings.Property(s => s.DefaultCountry).HasColumnName("Settings_DefaultCountry").HasMaxLength(100);
                    settings.Property(s => s.DefaultCity).HasColumnName("Settings_DefaultCity").HasMaxLength(100);
                    settings.Property(s => s.ThemeColor).HasColumnName("Settings_ThemeColor").HasMaxLength(10);
                    settings.Property(s => s.LogoUrl).HasColumnName("Settings_LogoUrl").HasMaxLength(500);
                    settings.Property(s => s.ItemsPerPage).HasColumnName("Settings_ItemsPerPage");
                    settings.Property(s => s.ShowGridLines).HasColumnName("Settings_ShowGridLines");
                    settings.Property(s => s.EmailNotificationsEnabled).HasColumnName("Settings_EmailNotificationsEnabled");
                    settings.Property(s => s.SmsNotificationsEnabled).HasColumnName("Settings_SmsNotificationsEnabled");
                    settings.Property(s => s.NotificationEmail).HasColumnName("Settings_NotificationEmail").HasMaxLength(256);
                    settings.Property(s => s.PasswordMinLength).HasColumnName("Settings_PasswordMinLength");
                    settings.Property(s => s.RequireTwoFactor).HasColumnName("Settings_RequireTwoFactor");
                    settings.Property(s => s.SessionTimeoutMinutes).HasColumnName("Settings_SessionTimeoutMinutes");
                    settings.Property(s => s.MaxLoginAttempts).HasColumnName("Settings_MaxLoginAttempts");
                    settings.Property(s => s.ModuleSettingsJson).HasColumnName("Settings_ModuleSettingsJson").HasColumnType("nvarchar(max)");

                    settings.Ignore(s => s.ModuleSettings);
                });

                // Configure DatabaseInfo as owned
                entity.OwnsOne(t => t.DatabaseInfo, dbInfo =>
                {
                    dbInfo.WithOwner();

                    dbInfo.Property(d => d.DatabaseName).HasColumnName("DB_Name").HasMaxLength(128);
                    dbInfo.Property(d => d.Server).HasColumnName("DB_Server").HasMaxLength(256);
                    dbInfo.Property(d => d.Username).HasColumnName("DB_Username").HasMaxLength(128);
                    dbInfo.Property(d => d.EncryptedPassword).HasColumnName("DB_EncryptedPassword").HasMaxLength(512);
                    dbInfo.Property(d => d.EncryptedConnectionString).HasColumnName("DB_EncryptedConnectionString").HasMaxLength(2048);
                    dbInfo.Property(d => d.Port).HasColumnName("DB_Port").HasDefaultValue(1433);
                    dbInfo.Property(d => d.UseWindowsAuthentication).HasColumnName("DB_UseWindowsAuth").HasDefaultValue(false);
                    dbInfo.Property(d => d.Encrypt).HasColumnName("DB_Encrypt").HasDefaultValue(true);
                    dbInfo.Property(d => d.TrustServerCertificate).HasColumnName("DB_TrustServerCert").HasDefaultValue(false);
                    dbInfo.Property(d => d.ConnectionTimeout).HasColumnName("DB_ConnectionTimeout").HasDefaultValue(30);
                    dbInfo.Property(d => d.CommandTimeout).HasColumnName("DB_CommandTimeout").HasDefaultValue(30);
                    dbInfo.Property(d => d.ApplicationName).HasColumnName("DB_ApplicationName").HasMaxLength(128);
                    dbInfo.Property(d => d.SizeInMB).HasColumnName("DB_SizeMB");
                    dbInfo.Property(d => d.MaxSizeInMB).HasColumnName("DB_MaxSizeMB");
                    dbInfo.Property(d => d.Collation).HasColumnName("DB_Collation").HasMaxLength(128).HasDefaultValue("SQL_Latin1_General_CP1_CI_AS");
                    dbInfo.Property(d => d.RecoveryModel).HasColumnName("DB_RecoveryModel").HasMaxLength(20).HasDefaultValue("FULL");
                    dbInfo.Property(d => d.CompatibilityLevel).HasColumnName("DB_CompatibilityLevel").HasMaxLength(10).HasDefaultValue("150");
                    dbInfo.Property(d => d.CreatedDate).HasColumnName("DB_CreatedDate");
                    dbInfo.Property(d => d.LastMigrationDate).HasColumnName("DB_LastMigrationDate");
                    dbInfo.Property(d => d.LastBackupDate).HasColumnName("DB_LastBackupDate");
                    dbInfo.Property(d => d.LastRestoreDate).HasColumnName("DB_LastRestoreDate");
                    dbInfo.Property(d => d.LastHealthCheckDate).HasColumnName("DB_LastHealthCheckDate");
                    dbInfo.Property(d => d.LastOptimizationDate).HasColumnName("DB_LastOptimizationDate");
                    dbInfo.Property(d => d.SchemaVersion).HasColumnName("DB_SchemaVersion").HasMaxLength(50);

                    dbInfo.Ignore(d => d.ActiveConnections);
                    dbInfo.Ignore(d => d.CpuUsagePercent);
                    dbInfo.Ignore(d => d.MemoryUsageMB);
                    dbInfo.Ignore(d => d.DiskIOReadMBPerSec);
                    dbInfo.Ignore(d => d.DiskIOWriteMBPerSec);
                });

                // Relationships
                entity.HasMany(t => t.ModuleSubscriptions)
                    .WithOne(s => s.Tenant)
                    .HasForeignKey(s => s.TenantId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(t => t.Users)
                    .WithOne(u => u.Tenant)
                    .HasForeignKey(u => u.TenantId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(t => t.Invoices)
                    .WithOne(i => i.Tenant)
                    .HasForeignKey(i => i.TenantId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void ApplyGlobalQueryFilters(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (entityType.IsOwned())
                    continue;

                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                    var filter = Expression.Lambda(
                        Expression.Equal(property, Expression.Constant(false)),
                        parameter
                    );

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
                }
            }
        }
    }
}
