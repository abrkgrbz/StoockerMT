using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StoockerMT.Persistence.Migrations.MasterDb
{
    /// <inheritdoc />
    public partial class InitialMasterDbWithValueObjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Modules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MonthlyPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MonthlyCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    YearlyPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    YearlyCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    Version = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "1.0.0"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Configuration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dependencies = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Settings_TimeZone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Settings_DateFormat = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Settings_TimeFormat = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Settings_Language = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Settings_Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Settings_FiscalYearStartMonth = table.Column<int>(type: "int", nullable: true),
                    Settings_FirstDayOfWeek = table.Column<int>(type: "int", nullable: true),
                    Settings_DefaultCountry = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Settings_DefaultCity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Settings_ThemeColor = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Settings_LogoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Settings_ItemsPerPage = table.Column<int>(type: "int", nullable: true),
                    Settings_ShowGridLines = table.Column<bool>(type: "bit", nullable: true),
                    Settings_EmailNotificationsEnabled = table.Column<bool>(type: "bit", nullable: true),
                    Settings_SmsNotificationsEnabled = table.Column<bool>(type: "bit", nullable: true),
                    Settings_NotificationEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Settings_PasswordMinLength = table.Column<int>(type: "int", nullable: true),
                    Settings_RequireTwoFactor = table.Column<bool>(type: "bit", nullable: true),
                    Settings_SessionTimeoutMinutes = table.Column<int>(type: "int", nullable: true),
                    Settings_MaxLoginAttempts = table.Column<int>(type: "int", nullable: true),
                    Settings_ModuleSettingsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DB_Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    DB_Server = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DB_Username = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    DB_EncryptedPassword = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    DB_EncryptedConnectionString = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    DB_Port = table.Column<int>(type: "int", nullable: true, defaultValue: 1433),
                    DB_UseWindowsAuth = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DB_Encrypt = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    DB_TrustServerCert = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    DB_ConnectionTimeout = table.Column<int>(type: "int", nullable: true, defaultValue: 30),
                    DB_CommandTimeout = table.Column<int>(type: "int", nullable: true, defaultValue: 30),
                    DB_ApplicationName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    DB_SizeMB = table.Column<long>(type: "bigint", nullable: true),
                    DB_MaxSizeMB = table.Column<int>(type: "int", nullable: true),
                    DB_Collation = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true, defaultValue: "SQL_Latin1_General_CP1_CI_AS"),
                    DB_RecoveryModel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "FULL"),
                    DB_CompatibilityLevel = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true, defaultValue: "150"),
                    DB_CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DB_LastMigrationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DB_LastBackupDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DB_LastRestoreDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DB_LastHealthCheckDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DB_LastOptimizationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DB_SchemaVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaxUsers = table.Column<int>(type: "int", nullable: false, defaultValue: 10),
                    MaxStorageBytes = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1073741824L),
                    MaxModules = table.Column<int>(type: "int", nullable: false, defaultValue: 5),
                    ActivatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeactivatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeactivationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModuleFeatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModuleId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Configuration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModuleFeatures_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModulePermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModuleId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModulePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModulePermissions_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantInvoices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BillingStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BillingEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubTotalCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PaymentReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantInvoices_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantModuleSubscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    ModuleId = table.Column<int>(type: "int", nullable: false),
                    SubscriptionStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubscriptionEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubscriptionType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DiscountCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    AutoRenew = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantModuleSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantModuleSubscriptions_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantModuleSubscriptions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsTenantAdmin = table.Column<bool>(type: "bit", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmailVerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PasswordChangeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailedLoginAttempts = table.Column<int>(type: "int", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    LockedUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantUsers_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantInvoiceItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceId = table.Column<int>(type: "int", nullable: false),
                    ModuleId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "PCS"),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantInvoiceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantInvoiceItems_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TenantInvoiceItems_TenantInvoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "TenantInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantModuleUsages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubscriptionId = table.Column<int>(type: "int", nullable: false),
                    Feature = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UsageCount = table.Column<int>(type: "int", nullable: false),
                    UsageDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MetaData = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantModuleUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantModuleUsages_TenantModuleSubscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "TenantModuleSubscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantUserPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantUserId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    GrantedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GrantedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantUserPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantUserPermissions_ModulePermissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "ModulePermissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantUserPermissions_TenantUsers_TenantUserId",
                        column: x => x.TenantUserId,
                        principalTable: "TenantUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Modules",
                columns: new[] { "Id", "Category", "Code", "Configuration", "CreatedAt", "CreatedBy", "Dependencies", "Description", "IsActive", "IsDeleted", "Name", "UpdatedAt", "UpdatedBy", "Version", "MonthlyPrice", "MonthlyCurrency", "YearlyPrice", "YearlyCurrency" },
                values: new object[,]
                {
                    { 1, 1, "CRM", null, new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, "Complete CRM solution for managing customers, leads, and sales", true, false, "Customer Relationship Management", null, null, "1.0.0", 99.99m, "USD", 999.99m, "USD" },
                    { 2, 2, "INV", null, new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, "Inventory and warehouse management system", true, false, "Inventory Management", null, null, "1.0.0", 149.99m, "USD", 1499.99m, "USD" },
                    { 3, 3, "ACC", null, new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, "Financial accounting and reporting system", true, false, "Accounting", null, null, "1.0.0", 199.99m, "USD", 1999.99m, "USD" },
                    { 4, 4, "HR", null, new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, "Employee management and HR processes", true, false, "Human Resources", null, null, "1.0.0", 129.99m, "USD", 1299.99m, "USD" }
                });

            migrationBuilder.InsertData(
                table: "ModuleFeatures",
                columns: new[] { "Id", "Code", "Configuration", "CreatedAt", "Description", "IsDeleted", "IsEnabled", "ModuleId", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "CRM_LEADS", null, new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Manage leads and opportunities", false, true, 1, "Lead Management", null },
                    { 2, "CRM_CONTACTS", null, new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Manage customer contacts", false, true, 1, "Contact Management", null },
                    { 3, "CRM_PIPELINE", null, new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Visual sales pipeline management", false, true, 1, "Sales Pipeline", null },
                    { 4, "INV_STOCK", null, new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Real-time stock tracking", false, true, 2, "Stock Tracking", null },
                    { 5, "INV_BARCODE", null, new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Barcode scanning support", false, true, 2, "Barcode Scanning", null },
                    { 6, "INV_REORDER", null, new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Automatic reorder points", false, true, 2, "Reorder Management", null },
                    { 7, "ACC_GL", null, new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), "General ledger management", false, true, 3, "General Ledger", null },
                    { 8, "ACC_REPORTS", null, new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Financial reporting suite", false, true, 3, "Financial Reports", null },
                    { 9, "ACC_TAX", null, new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Tax calculation and reporting", false, true, 3, "Tax Management", null },
                    { 10, "HR_LEAVE", null, new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Employee leave management", false, true, 4, "Leave Management", null },
                    { 11, "HR_TIMESHEET", null, new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Employee timesheet tracking", false, true, 4, "Timesheet", null },
                    { 12, "HR_PAYROLL", null, new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Payroll processing", false, true, 4, "Payroll", null }
                });

            migrationBuilder.InsertData(
                table: "ModulePermissions",
                columns: new[] { "Id", "Code", "CreatedAt", "Description", "IsDeleted", "ModuleId", "Name", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "CRM_VIEW_CUSTOMERS", new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 1, "View Customers", 1, null },
                    { 2, "CRM_MANAGE_CUSTOMERS", new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 1, "Manage Customers", 2, null },
                    { 3, "CRM_DELETE_CUSTOMERS", new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 1, "Delete Customers", 3, null },
                    { 4, "CRM_ADMIN", new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 1, "CRM Admin", 4, null },
                    { 5, "INV_VIEW_PRODUCTS", new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 2, "View Products", 1, null },
                    { 6, "INV_MANAGE_PRODUCTS", new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 2, "Manage Products", 2, null },
                    { 7, "INV_MANAGE_INVENTORY", new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 2, "Manage Inventory", 2, null },
                    { 8, "INV_ADMIN", new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 2, "Inventory Admin", 4, null },
                    { 9, "ACC_VIEW_ACCOUNTS", new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 3, "View Accounts", 1, null },
                    { 10, "ACC_MANAGE_ACCOUNTS", new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 3, "Manage Accounts", 2, null },
                    { 11, "ACC_VIEW_REPORTS", new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 3, "View Reports", 1, null },
                    { 12, "ACC_ADMIN", new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 3, "Accounting Admin", 4, null },
                    { 13, "HR_VIEW_EMPLOYEES", new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 4, "View Employees", 1, null },
                    { 14, "HR_MANAGE_EMPLOYEES", new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 4, "Manage Employees", 2, null },
                    { 15, "HR_MANAGE_PAYROLL", new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 4, "Manage Payroll", 2, null },
                    { 16, "HR_ADMIN", new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 4, "HR Admin", 4, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModuleFeatures_ModuleId_Code",
                table: "ModuleFeatures",
                columns: new[] { "ModuleId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModulePermissions_ModuleId_Code",
                table: "ModulePermissions",
                columns: new[] { "ModuleId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Modules_Code",
                table: "Modules",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantInvoiceItems_InvoiceId",
                table: "TenantInvoiceItems",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantInvoiceItems_ModuleId",
                table: "TenantInvoiceItems",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantInvoices_DueDate",
                table: "TenantInvoices",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_TenantInvoices_InvoiceNumber",
                table: "TenantInvoices",
                column: "InvoiceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantInvoices_TenantStatus",
                table: "TenantInvoices",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TenantModuleSubscriptions_ModuleId",
                table: "TenantModuleSubscriptions",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantModuleSubscriptions_SubscriptionEndDate",
                table: "TenantModuleSubscriptions",
                column: "SubscriptionEndDate");

            migrationBuilder.CreateIndex(
                name: "IX_TenantModuleSubscriptions_TenantId_ModuleId",
                table: "TenantModuleSubscriptions",
                columns: new[] { "TenantId", "ModuleId" });

            migrationBuilder.CreateIndex(
                name: "IX_TenantModuleUsages_Subscription_Date",
                table: "TenantModuleUsages",
                columns: new[] { "SubscriptionId", "UsageDate" });

            migrationBuilder.CreateIndex(
                name: "IX_TenantModuleUsages_UsageDate",
                table: "TenantModuleUsages",
                column: "UsageDate");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Code",
                table: "Tenants",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantUserPermissions_PermissionId",
                table: "TenantUserPermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUserPermissions_User_Permission",
                table: "TenantUserPermissions",
                columns: new[] { "TenantUserId", "PermissionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantUsers_Email",
                table: "TenantUsers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantUsers_TenantId",
                table: "TenantUsers",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModuleFeatures");

            migrationBuilder.DropTable(
                name: "TenantInvoiceItems");

            migrationBuilder.DropTable(
                name: "TenantModuleUsages");

            migrationBuilder.DropTable(
                name: "TenantUserPermissions");

            migrationBuilder.DropTable(
                name: "TenantInvoices");

            migrationBuilder.DropTable(
                name: "TenantModuleSubscriptions");

            migrationBuilder.DropTable(
                name: "ModulePermissions");

            migrationBuilder.DropTable(
                name: "TenantUsers");

            migrationBuilder.DropTable(
                name: "Modules");

            migrationBuilder.DropTable(
                name: "Tenants");
        }
    }
}
