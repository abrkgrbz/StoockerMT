using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;
using StoockerMT.Application.Common.Interfaces.Services;
using StoockerMT.Domain.Entities.MasterDb;

namespace StoockerMT.Persistence.Configurations.MasterDb
{
    public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
    {
        private readonly IServiceProvider _serviceProvider;

        public TenantConfiguration(IServiceProvider serviceProvider = null)
        {
            _serviceProvider = serviceProvider;
        }

        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            builder.ToTable("Tenants");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Description)
                .HasMaxLength(1000);

            builder.Property(t => t.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(t => t.CreatedBy)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.UpdatedBy)
                .HasMaxLength(100);

            builder.Property(t => t.CreatedAt)
                .IsRequired();

            builder.Property(t => t.UpdatedAt);

            builder.Property(t => t.MaxUsers)
                .HasDefaultValue(10)
                .IsRequired();

            builder.Property(t => t.MaxStorageBytes)
                .HasDefaultValue(1073741824L)
                .IsRequired();

            builder.Property(t => t.MaxModules)
                .HasDefaultValue(5)
                .IsRequired();

            builder.Property(t => t.ActivatedDate);
            builder.Property(t => t.DeactivatedDate);
            builder.Property(t => t.DeactivationReason)
                .HasMaxLength(500);

            // Configure TenantCode as owned
            builder.OwnsOne(t => t.Code, code =>
            {
                code.Property(c => c.Value)
                    .HasColumnName("Code")
                    .IsRequired()
                    .HasMaxLength(50);

                code.HasIndex(c => c.Value)
                    .IsUnique()
                    .HasDatabaseName("IX_Tenants_Code");
            });

            // Configure TenantSettings as owned
            builder.OwnsOne(t => t.Settings, settings =>
            {
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

            // Configure DatabaseInfo as owned with encryption
            builder.OwnsOne(t => t.DatabaseInfo, dbInfo =>
            {
                dbInfo.Property(d => d.DatabaseName).HasColumnName("DB_Name").HasMaxLength(128);
                dbInfo.Property(d => d.Server).HasColumnName("DB_Server").HasMaxLength(256);
                dbInfo.Property(d => d.Username).HasColumnName("DB_Username").HasMaxLength(128);

                // Get encryption service from service provider if available
                if (_serviceProvider != null)
                {
                    var encryptionService = _serviceProvider.GetService<IEncryptionService>();
                    if (encryptionService != null)
                    {
                        dbInfo.Property(d => d.EncryptedPassword)
                            .HasColumnName("DB_EncryptedPassword")
                            .HasMaxLength(512)
                            .HasConversion(new EncryptionValueConverter(encryptionService));

                        dbInfo.Property(d => d.EncryptedConnectionString)
                            .HasColumnName("DB_EncryptedConnectionString")
                            .HasMaxLength(2048)
                            .HasConversion(new EncryptionValueConverter(encryptionService));
                    }
                    else
                    {
                        // Fallback to DataProtection API
                        var dataProtector = _serviceProvider.GetService<IDataProtector>();
                        if (dataProtector != null)
                        {
                            dbInfo.Property(d => d.EncryptedPassword)
                                .HasColumnName("DB_EncryptedPassword")
                                .HasMaxLength(512)
                                .HasConversion(new DataProtectionValueConverter(dataProtector));

                            dbInfo.Property(d => d.EncryptedConnectionString)
                                .HasColumnName("DB_EncryptedConnectionString")
                                .HasMaxLength(2048)
                                .HasConversion(new DataProtectionValueConverter(dataProtector));
                        }
                        else
                        {
                            // No encryption available - log warning
                            dbInfo.Property(d => d.EncryptedPassword)
                                .HasColumnName("DB_EncryptedPassword")
                                .HasMaxLength(512);

                            dbInfo.Property(d => d.EncryptedConnectionString)
                                .HasColumnName("DB_EncryptedConnectionString")
                                .HasMaxLength(2048);
                        }
                    }
                }
                else
                {
                    // No service provider - use plain storage
                    dbInfo.Property(d => d.EncryptedPassword)
                        .HasColumnName("DB_EncryptedPassword")
                        .HasMaxLength(512);

                    dbInfo.Property(d => d.EncryptedConnectionString)
                        .HasColumnName("DB_EncryptedConnectionString")
                        .HasMaxLength(2048);
                }

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

                // Ignore transient properties
                dbInfo.Ignore(d => d.ActiveConnections);
                dbInfo.Ignore(d => d.CpuUsagePercent);
                dbInfo.Ignore(d => d.MemoryUsageMB);
                dbInfo.Ignore(d => d.DiskIOReadMBPerSec);
                dbInfo.Ignore(d => d.DiskIOWriteMBPerSec);
            });

            // Relationships
            builder.HasMany(t => t.ModuleSubscriptions)
                .WithOne(s => s.Tenant)
                .HasForeignKey(s => s.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.Users)
                .WithOne(u => u.Tenant)
                .HasForeignKey(u => u.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.Invoices)
                .WithOne(i => i.Tenant)
                .HasForeignKey(i => i.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        // Value converter using IEncryptionService
        public class EncryptionValueConverter : ValueConverter<string, string>
        {
            public EncryptionValueConverter(IEncryptionService encryptionService)
                : base(
                    v => Encrypt(v, encryptionService),
                    v => Decrypt(v, encryptionService))
            {
            }

            private static string Encrypt(string value, IEncryptionService encryptionService)
            {
                return string.IsNullOrEmpty(value) ? value : encryptionService.Encrypt(value);
            }

            private static string Decrypt(string value, IEncryptionService encryptionService)
            {
                return string.IsNullOrEmpty(value) ? value : encryptionService.Decrypt(value);
            }
        }

        // Fallback value converter using IDataProtector
        public class DataProtectionValueConverter : ValueConverter<string, string>
        {
            public DataProtectionValueConverter(IDataProtector dataProtector)
                : base(
                    v => Encrypt(v, dataProtector),
                    v => Decrypt(v, dataProtector))
            {
            }

            private static string Encrypt(string value, IDataProtector protector)
            {
                return string.IsNullOrEmpty(value) ? value : protector.Protect(value);
            }

            private static string Decrypt(string value, IDataProtector protector)
            {
                if (string.IsNullOrEmpty(value)) return value;

                try
                {
                    return protector.Unprotect(value);
                }
                catch
                {
                    // If decryption fails, return original value (for backward compatibility)
                    return value;
                }
            }
        }
    }
}