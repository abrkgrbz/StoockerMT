using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StoockerMT.Domain.Entities.MasterDb;

namespace StoockerMT.Persistence.Configurations.MasterDb
{
    public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
    {
        private readonly IDataProtector _dataProtector;

        public TenantConfiguration(IDataProtector dataProtector = null)
        {
            _dataProtector = dataProtector;
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
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            builder.Property(t => t.UpdatedAt);

            builder.Property(t => t.MaxUsers)
                .HasDefaultValue(10)
                .IsRequired();

            builder.Property(t => t.MaxStorageBytes)
                .HasDefaultValue(1073741824L)
                .IsRequired();

            // Add missing properties
            builder.Property(t => t.MaxModules)
                .HasDefaultValue(5)
                .IsRequired();

            builder.Property(t => t.ActivatedDate);

            builder.Property(t => t.DeactivatedDate);

            builder.Property(t => t.DeactivationReason)
                .HasMaxLength(500);

            // Value Object: TenantCode
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

            // Value Object: TenantSettings
            builder.OwnsOne(t => t.Settings, settings =>
            { 
                settings.Property(s => s.TimeZone)
                    .HasColumnName("Settings_TimeZone")
                    .HasMaxLength(50)
                    .IsRequired(false); // Make nullable since Settings itself is nullable

                settings.Property(s => s.DateFormat)
                    .HasColumnName("Settings_DateFormat")
                    .HasMaxLength(20)
                    .IsRequired(false);

                settings.Property(s => s.TimeFormat)
                    .HasColumnName("Settings_TimeFormat")
                    .HasMaxLength(20)
                    .IsRequired(false);

                settings.Property(s => s.Language)
                    .HasColumnName("Settings_Language")
                    .HasMaxLength(10)
                    .IsRequired(false);

                settings.Property(s => s.Currency)
                    .HasColumnName("Settings_Currency")
                    .HasMaxLength(3)
                    .IsRequired(false);

                settings.Property(s => s.FiscalYearStartMonth)
                    .HasColumnName("Settings_FiscalYearStartMonth")
                    .IsRequired(false);

                settings.Property(s => s.FirstDayOfWeek)
                    .HasColumnName("Settings_FirstDayOfWeek")
                    .IsRequired(false);

                settings.Property(s => s.DefaultCountry)
                    .HasColumnName("Settings_DefaultCountry")
                    .HasMaxLength(100)
                    .IsRequired(false);

                settings.Property(s => s.DefaultCity)
                    .HasColumnName("Settings_DefaultCity")
                    .HasMaxLength(100)
                    .IsRequired(false);

                settings.Property(s => s.ThemeColor)
                    .HasColumnName("Settings_ThemeColor")
                    .HasMaxLength(10)
                    .IsRequired(false);

                settings.Property(s => s.LogoUrl)
                    .HasColumnName("Settings_LogoUrl")
                    .HasMaxLength(500)
                    .IsRequired(false);

                settings.Property(s => s.ItemsPerPage)
                    .HasColumnName("Settings_ItemsPerPage")
                    .IsRequired(false);

                settings.Property(s => s.ShowGridLines)
                    .HasColumnName("Settings_ShowGridLines")
                    .IsRequired(false);

                settings.Property(s => s.EmailNotificationsEnabled)
                    .HasColumnName("Settings_EmailNotificationsEnabled")
                    .IsRequired(false);

                settings.Property(s => s.SmsNotificationsEnabled)
                    .HasColumnName("Settings_SmsNotificationsEnabled")
                    .IsRequired(false);

                settings.Property(s => s.NotificationEmail)
                    .HasColumnName("Settings_NotificationEmail")
                    .HasMaxLength(256)
                    .IsRequired(false);

                settings.Property(s => s.PasswordMinLength)
                    .HasColumnName("Settings_PasswordMinLength")
                    .IsRequired(false);

                settings.Property(s => s.RequireTwoFactor)
                    .HasColumnName("Settings_RequireTwoFactor")
                    .IsRequired(false);

                settings.Property(s => s.SessionTimeoutMinutes)
                    .HasColumnName("Settings_SessionTimeoutMinutes")
                    .IsRequired(false);

                settings.Property(s => s.MaxLoginAttempts)
                    .HasColumnName("Settings_MaxLoginAttempts")
                    .IsRequired(false);

                settings.Property(s => s.ModuleSettingsJson)
                    .HasColumnName("Settings_ModuleSettingsJson")
                    .HasColumnType("nvarchar(max)")
                    .IsRequired(false);

                settings.Ignore(s => s.ModuleSettings);
            });

            // Value Object: DatabaseInfo
            builder.OwnsOne(t => t.DatabaseInfo, dbInfo =>
            { 
                dbInfo.Property(d => d.DatabaseName)
                    .HasColumnName("DB_Name")
                    .HasMaxLength(128)
                    .IsRequired(false);

                dbInfo.Property(d => d.Server)
                    .HasColumnName("DB_Server")
                    .HasMaxLength(256)
                    .IsRequired(false);

                dbInfo.Property(d => d.Username)
                    .HasColumnName("DB_Username")
                    .HasMaxLength(128)
                    .IsRequired(false);
                 
                if (_dataProtector != null)
                {
                    dbInfo.Property(d => d.EncryptedPassword)
                        .HasColumnName("DB_EncryptedPassword")
                        .HasMaxLength(512)
                        .HasConversion(new EncryptionValueConverter(_dataProtector))
                        .IsRequired(false);

                    dbInfo.Property(d => d.EncryptedConnectionString)
                        .HasColumnName("DB_EncryptedConnectionString")
                        .HasMaxLength(2048)
                        .HasConversion(new EncryptionValueConverter(_dataProtector))
                        .IsRequired(false);
                }
                else
                {
                    dbInfo.Property(d => d.EncryptedPassword)
                        .HasColumnName("DB_EncryptedPassword")
                        .HasMaxLength(512)
                        .IsRequired(false);

                    dbInfo.Property(d => d.EncryptedConnectionString)
                        .HasColumnName("DB_EncryptedConnectionString")
                        .HasMaxLength(2048)
                        .IsRequired(false);
                }

                dbInfo.Property(d => d.Port)
                    .HasColumnName("DB_Port")
                    .HasDefaultValue(1433)
                    .IsRequired(false);

                dbInfo.Property(d => d.UseWindowsAuthentication)
                    .HasColumnName("DB_UseWindowsAuth")
                    .HasDefaultValue(false)
                    .IsRequired(false);

                dbInfo.Property(d => d.Encrypt)
                    .HasColumnName("DB_Encrypt")
                    .HasDefaultValue(true)
                    .IsRequired(false);

                dbInfo.Property(d => d.TrustServerCertificate)
                    .HasColumnName("DB_TrustServerCert")
                    .HasDefaultValue(false)
                    .IsRequired(false);

                dbInfo.Property(d => d.ConnectionTimeout)
                    .HasColumnName("DB_ConnectionTimeout")
                    .HasDefaultValue(30)
                    .IsRequired(false);

                dbInfo.Property(d => d.CommandTimeout)
                    .HasColumnName("DB_CommandTimeout")
                    .HasDefaultValue(30)
                    .IsRequired(false);

                dbInfo.Property(d => d.ApplicationName)
                    .HasColumnName("DB_ApplicationName")
                    .HasMaxLength(128)
                    .IsRequired(false);

                dbInfo.Property(d => d.SizeInMB)
                    .HasColumnName("DB_SizeMB")
                    .IsRequired(false);

                dbInfo.Property(d => d.MaxSizeInMB)
                    .HasColumnName("DB_MaxSizeMB")
                    .IsRequired(false);

                dbInfo.Property(d => d.Collation)
                    .HasColumnName("DB_Collation")
                    .HasMaxLength(128)
                    .HasDefaultValue("SQL_Latin1_General_CP1_CI_AS")
                    .IsRequired(false);

                dbInfo.Property(d => d.RecoveryModel)
                    .HasColumnName("DB_RecoveryModel")
                    .HasMaxLength(20)
                    .HasDefaultValue("FULL")
                    .IsRequired(false);

                dbInfo.Property(d => d.CompatibilityLevel)
                    .HasColumnName("DB_CompatibilityLevel")
                    .HasMaxLength(10)
                    .HasDefaultValue("150")
                    .IsRequired(false);

                dbInfo.Property(d => d.CreatedDate)
                    .HasColumnName("DB_CreatedDate")
                    .IsRequired(false);

                dbInfo.Property(d => d.LastMigrationDate)
                    .HasColumnName("DB_LastMigrationDate")
                    .IsRequired(false);

                dbInfo.Property(d => d.LastBackupDate)
                    .HasColumnName("DB_LastBackupDate")
                    .IsRequired(false);

                dbInfo.Property(d => d.LastRestoreDate)
                    .HasColumnName("DB_LastRestoreDate")
                    .IsRequired(false);

                dbInfo.Property(d => d.LastHealthCheckDate)
                    .HasColumnName("DB_LastHealthCheckDate")
                    .IsRequired(false);

                dbInfo.Property(d => d.LastOptimizationDate)
                    .HasColumnName("DB_LastOptimizationDate")
                    .IsRequired(false);

                dbInfo.Property(d => d.SchemaVersion)
                    .HasColumnName("DB_SchemaVersion")
                    .HasMaxLength(50)
                    .IsRequired(false);

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

        // Nested class for encryption
        public class EncryptionValueConverter : ValueConverter<string, string>
        {
            public EncryptionValueConverter(IDataProtector dataProtector)
                : base(
                    v => Encrypt(v, dataProtector),
                    v => Decrypt(v, dataProtector))
            {
            }

            private static string Encrypt(string value, IDataProtector protector)
            {
                if (string.IsNullOrEmpty(value)) return value;

                if (protector == null)
                    return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));

                return protector.Protect(value);
            }

            private static string Decrypt(string value, IDataProtector protector)
            {
                if (string.IsNullOrEmpty(value)) return value;

                if (protector == null)
                    return Encoding.UTF8.GetString(Convert.FromBase64String(value));

                return protector.Unprotect(value);
            }
        }
    }
}