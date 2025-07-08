using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StoockerMT.Application.Common.Interfaces.Services;
using StoockerMT.Domain.Entities.MasterDb;
using StoockerMT.Domain.Enums;
using StoockerMT.Domain.Repositories.UnitOfWork;
using StoockerMT.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Domain.Extensions;
using StoockerMT.Domain.ValueObjects;

namespace StoockerMT.Persistence.Services
{
    public class TenantService : ITenantService
    {
        private readonly IMasterDbUnitOfWork _masterDbUnitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TenantService> _logger;
        private readonly string _masterConnectionString;
        private readonly string _backupPath;

        public TenantService(
            IMasterDbUnitOfWork masterDbUnitOfWork,
            IConfiguration configuration,
            ILogger<TenantService> logger)
        {
            _masterDbUnitOfWork = masterDbUnitOfWork;
            _configuration = configuration;
            _logger = logger;
            _masterConnectionString = Configuration.ConnectionStringMasterDb;
            _backupPath = configuration["BackupSettings:Path"] ?? @"C:\Backups";
        }

        public async Task<string> CreateTenantDatabaseAsync(int tenantId, string tenantCode, CancellationToken cancellationToken = default)
        {
            var databaseName = $"TenantDB_{tenantCode}_{tenantId}";

            try
            {
                _logger.LogInformation("Creating database for tenant {TenantId} with code {TenantCode}", tenantId, tenantCode);
                 
                await _masterDbUnitOfWork.BeginTransactionAsync(cancellationToken);
                 
                var tenant = await _masterDbUnitOfWork.Tenants.GetByIdAsync(tenantId, cancellationToken);
                if (tenant == null)
                {
                    throw new InvalidOperationException($"Tenant with ID {tenantId} not found");
                }
                 
                if (tenant.Code.Value != tenantCode)
                {
                    throw new InvalidOperationException($"Tenant code mismatch. Expected: {tenant.Code.Value}, Provided: {tenantCode}");
                }
                 
                if (await DatabaseExistsAsync(databaseName, cancellationToken))
                {
                    throw new InvalidOperationException($"Database {databaseName} already exists");
                }
                 
                await CreateDatabaseAsync(databaseName, cancellationToken);
                 
                var (username, password) = await CreateDatabaseUserAsync(databaseName, tenantCode, cancellationToken);
                 
                var connectionString = BuildTenantConnectionString(databaseName, username, password);
                var encryptedPassword = EncryptPassword(password);
                var encryptedConnectionString = EncryptConnectionString(connectionString);

                var databaseInfo = DatabaseInfo.Create(
                    databaseName: databaseName,
                    server: _configuration["DatabaseSettings:Server"] ?? "localhost",
                    username: username,
                    encryptedPassword: encryptedPassword,
                    encryptedConnectionString: encryptedConnectionString,
                    port: int.Parse(_configuration["DatabaseSettings:Port"] ?? "1433"),
                    useWindowsAuthentication: false,
                    encrypt: bool.Parse(_configuration["DatabaseSettings:Encrypt"] ?? "true"),
                    trustServerCertificate: bool.Parse(_configuration["DatabaseSettings:TrustServerCertificate"] ?? "false"),
                    connectionTimeout: int.Parse(_configuration["DatabaseSettings:ConnectionTimeout"] ?? "30"),
                    commandTimeout: int.Parse(_configuration["DatabaseSettings:CommandTimeout"] ?? "30"),
                    applicationName: $"TenantApp_{tenantCode}",
                    collation: _configuration["DatabaseSettings:Collation"] ?? "SQL_Latin1_General_CP1_CI_AS",
                    recoveryModel: _configuration["DatabaseSettings:RecoveryModel"] ?? "FULL",
                    compatibilityLevel: _configuration["DatabaseSettings:CompatibilityLevel"] ?? "150"
                );

                tenant.SetDatabaseInfo(databaseInfo);


                if (tenant.Settings == null)
                {
                    tenant.UpdateSettings(TenantSettings.Create(
                        timeZone: "UTC",
                        dateFormat: "yyyy-MM-dd",
                        language: "en-US",
                        currency: "USD"
                    ));
                }
                 
                if (tenant.Status == TenantStatus.Pending)
                {
                    tenant.Activate("System");
                }
                 
                _masterDbUnitOfWork.Tenants.Update(tenant);
                 
                await _masterDbUnitOfWork.CommitTransactionAsync(cancellationToken);
                await _masterDbUnitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully created database {DatabaseName} for tenant {TenantId}", databaseName, tenantId);

                return connectionString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create database for tenant {TenantId}", tenantId);
                 
                await _masterDbUnitOfWork.RollbackTransactionAsync(cancellationToken);
                 
                await RollbackDatabaseCreationAsync(databaseName, cancellationToken);

                throw;
            }
        }

        public async Task<bool> MigrateTenantDatabaseAsync(int tenantId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Starting migration for tenant {TenantId}", tenantId);
                 
                var tenant = await _masterDbUnitOfWork.Tenants.GetByIdAsync(tenantId, cancellationToken);
                if (tenant == null || tenant.DatabaseInfo == null)
                {
                    _logger.LogWarning("No database found for tenant {TenantId}", tenantId);
                    return false;
                }
                var originalConnectionString = tenant.DatabaseInfo.GetDecryptedConnectionString();
                var modifiedConnectionString = ForceSSLSettings(originalConnectionString);

                var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
                optionsBuilder.UseSqlServer(modifiedConnectionString);

                using var context = new TenantDbContext(optionsBuilder.Options);
                 
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync(cancellationToken);
                if (!pendingMigrations.Any())
                {
                    _logger.LogInformation("No pending migrations for tenant {TenantId}", tenantId);
                    return true;
                }
                 
                await context.Database.MigrateAsync(cancellationToken);
                 
                tenant.UpdateLastMigrationDate(DateTime.UtcNow);
                  _masterDbUnitOfWork.Tenants.Update(tenant);
                await _masterDbUnitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully migrated database for tenant {TenantId}", tenantId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to migrate database for tenant {TenantId}", tenantId);
                return false;
            }
        }

        public async Task<bool> DeleteTenantDatabaseAsync(int tenantId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogWarning("Starting database deletion for tenant {TenantId}", tenantId);

                await _masterDbUnitOfWork.BeginTransactionAsync(cancellationToken);

                // 1. Get tenant information
                var tenant = await _masterDbUnitOfWork.Tenants.GetByIdAsync(tenantId, cancellationToken);
                if (tenant == null || tenant.DatabaseInfo == null)
                {
                    _logger.LogWarning("No database found for tenant {TenantId}", tenantId);
                    return false;
                }

                // 2. Create a backup before deletion (safety measure)
                var backupCreated = await BackupTenantDatabaseAsync(tenantId, cancellationToken);
                if (!backupCreated)
                {
                    _logger.LogWarning("Could not create backup before deletion. Proceeding with caution.");
                }

                // 3. Deactivate all subscriptions
                var subscriptions = await _masterDbUnitOfWork.Subscriptions.GetByTenantAsync(tenantId, cancellationToken);
                foreach (var subscription in subscriptions)
                {
                    subscription.Cancel("Database deletion");
                     _masterDbUnitOfWork.Subscriptions.Update(subscription);
                }

                // 4. Close all connections to the database
                await CloseAllConnectionsAsync(tenant.DatabaseInfo.DatabaseName, cancellationToken);

                // 5. Drop the database
                await DropDatabaseAsync(tenant.DatabaseInfo.DatabaseName, cancellationToken);

                // 6. Drop the login (extract username from connection string)
                var username = ExtractUsernameFromConnectionString(tenant.DatabaseInfo.GetDecryptedConnectionString());
                if (!string.IsNullOrEmpty(username))
                {
                    await DropLoginAsync(username, cancellationToken);
                }

                // 7. Update tenant status
                tenant.Deactivate("Database deleted");
                tenant.ClearDatabaseInfo();

                 _masterDbUnitOfWork.Tenants.Update(tenant);
                await _masterDbUnitOfWork.CommitTransactionAsync(cancellationToken);
                await _masterDbUnitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully deleted database for tenant {TenantId}", tenantId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete database for tenant {TenantId}", tenantId);
                await _masterDbUnitOfWork.RollbackTransactionAsync(cancellationToken);
                return false;
            }
        }

        public async Task<bool> BackupTenantDatabaseAsync(int tenantId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Starting backup for tenant {TenantId}", tenantId);

                // 1. Get tenant information
                var tenant = await _masterDbUnitOfWork.Tenants.GetByIdAsync(tenantId, cancellationToken);
                if (tenant == null || tenant.DatabaseInfo == null)
                {
                    _logger.LogWarning("No database found for tenant {TenantId}", tenantId);
                    return false;
                }

                // 2. Create backup directory if not exists
                var backupDirectory = Path.Combine(_backupPath, $"Tenant_{tenantId}");
                Directory.CreateDirectory(backupDirectory);

                // 3. Generate backup file name
                var backupFileName = $"{tenant.DatabaseInfo.DatabaseName}_Backup_{DateTime.UtcNow:yyyyMMdd_HHmmss}.bak";
                var backupFilePath = Path.Combine(backupDirectory, backupFileName);

                // 4. Execute backup command
                var sql = $@"
                    BACKUP DATABASE [{tenant.DatabaseInfo.DatabaseName}] 
                    TO DISK = @BackupPath 
                    WITH FORMAT, INIT, 
                    NAME = @BackupName, 
                    SKIP, NOREWIND, NOUNLOAD, 
                    COMPRESSION, STATS = 10, 
                    CHECKSUM";

                using var connection = new SqlConnection(_masterConnectionString);
                await connection.OpenAsync(cancellationToken);

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@BackupPath", backupFilePath);
                command.Parameters.AddWithValue("@BackupName", $"Full Backup of {tenant.DatabaseInfo.DatabaseName}");
                command.CommandTimeout = 300; // 5 minutes timeout for large databases

                await command.ExecuteNonQueryAsync(cancellationToken);

                // 5. Verify backup file
                if (!File.Exists(backupFilePath))
                {
                    throw new InvalidOperationException("Backup file was not created");
                }

                // 6. Update tenant's last backup date
                tenant.UpdateLastBackupDate(DateTime.UtcNow);
                 _masterDbUnitOfWork.Tenants.Update(tenant);
                await _masterDbUnitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully created backup for tenant {TenantId} at {BackupPath}", tenantId, backupFilePath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to backup database for tenant {TenantId}", tenantId);
                return false;
            }
        }

        public async Task<bool> RestoreTenantDatabaseAsync(int tenantId, string backupPath, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Starting restore for tenant {TenantId} from {BackupPath}", tenantId, backupPath);

                // 1. Validate backup file exists
                if (!File.Exists(backupPath))
                {
                    _logger.LogError("Backup file not found: {BackupPath}", backupPath);
                    return false;
                }

                // 2. Get tenant information
                var tenant = await _masterDbUnitOfWork.Tenants.GetByIdAsync(tenantId, cancellationToken);
                if (tenant == null || tenant.DatabaseInfo == null)
                {
                    _logger.LogWarning("No database found for tenant {TenantId}", tenantId);
                    return false;
                }

                // 3. Close all connections to the database
                await CloseAllConnectionsAsync(tenant.DatabaseInfo.DatabaseName, cancellationToken);

                // 4. Execute restore command
                var sql = $@"
                    ALTER DATABASE [{tenant.DatabaseInfo.DatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    
                    RESTORE DATABASE [{tenant.DatabaseInfo.DatabaseName}] 
                    FROM DISK = @BackupPath 
                    WITH REPLACE, STATS = 10, CHECKSUM;
                    
                    ALTER DATABASE [{tenant.DatabaseInfo.DatabaseName}] SET MULTI_USER;";

                using var connection = new SqlConnection(_masterConnectionString);
                await connection.OpenAsync(cancellationToken);

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@BackupPath", backupPath);
                command.CommandTimeout = 600; // 10 minutes timeout for restore

                await command.ExecuteNonQueryAsync(cancellationToken);

                // 5. Verify database is accessible
                var isHealthy = await CheckTenantDatabaseHealthAsync(tenantId, cancellationToken);
                if (!isHealthy)
                {
                    throw new InvalidOperationException("Database restore completed but health check failed");
                }

                // 6. Update tenant's last restore date
                tenant.UpdateLastRestoreDate(DateTime.UtcNow);
                 _masterDbUnitOfWork.Tenants.Update(tenant);
                await _masterDbUnitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully restored database for tenant {TenantId} from {BackupPath}", tenantId, backupPath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to restore database for tenant {TenantId} from {BackupPath}", tenantId, backupPath);
                return false;
            }
        }

        public async Task<bool> CheckTenantDatabaseHealthAsync(int tenantId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Checking database health for tenant {TenantId}", tenantId);

                // 1. Get tenant information
                var tenant = await _masterDbUnitOfWork.Tenants.GetByIdAsync(tenantId, cancellationToken);
                if (tenant == null || tenant.DatabaseInfo == null)
                {
                    _logger.LogWarning("No database found for tenant {TenantId}", tenantId);
                    return false;
                }

                var originalConnectionString = tenant.DatabaseInfo.GetDecryptedConnectionString();
                var modifiedConnectionString = ForceSSLSettings(originalConnectionString);
                using var connection = new SqlConnection(modifiedConnectionString);
                await connection.OpenAsync(cancellationToken);

                // 3. Run health check queries
                var healthChecks = new[]
                {
                    // Check database is online
                    "SELECT DATABASEPROPERTYEX(DB_NAME(), 'Status') AS Status",
                    
                    // Check tables exist
                    "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'",
                    
                    // Check can read and write
                    @"BEGIN TRANSACTION;
                      DECLARE @TestValue NVARCHAR(50) = NEWID();
                      SELECT @TestValue;
                      ROLLBACK TRANSACTION;"
                };

                foreach (var checkSql in healthChecks)
                {
                    using var command = new SqlCommand(checkSql, connection);
                    command.CommandTimeout = 10; // Quick timeout for health checks

                    var result = await command.ExecuteScalarAsync(cancellationToken);
                    if (result == null)
                    {
                        _logger.LogWarning("Health check failed for tenant {TenantId}: Query returned null", tenantId);
                        return false;
                    }
                }
                 
                tenant.UpdateLastHealthCheckDate(DateTime.UtcNow);
                 
                if (tenant.Status == TenantStatus.Suspended)
                {
                    tenant.Activate("");
                }

                _masterDbUnitOfWork.Tenants.Update(tenant);
                await _masterDbUnitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogDebug("Database health check passed for tenant {TenantId}", tenantId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database health check failed for tenant {TenantId}", tenantId);
                return false;
            }
        }
        private string ForceSSLSettings(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
             
            builder.TrustServerCertificate = true; 

            return builder.ConnectionString;
        }
        // Helper Methods
        private async Task<bool> DatabaseExistsAsync(string databaseName, CancellationToken cancellationToken)
        {
            var sql = "SELECT database_id FROM sys.databases WHERE name = @DatabaseName";

            using var connection = new SqlConnection(_masterConnectionString);
            await connection.OpenAsync(cancellationToken);

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@DatabaseName", databaseName);

            var result = await command.ExecuteScalarAsync(cancellationToken);
            return result != null;
        }

        private async Task CreateDatabaseAsync(string databaseName, CancellationToken cancellationToken)
        {
            var dataPath = _configuration["DatabaseSettings:DataPath"] ?? @"C:\SQLData";
            var logPath = _configuration["DatabaseSettings:LogPath"] ?? @"C:\SQLLogs";

            var sql = $@"
                CREATE DATABASE [{databaseName}]
                CONTAINMENT = NONE
                ON PRIMARY 
                (
                    NAME = N'{databaseName}', 
                    FILENAME = N'{Path.Combine(dataPath, $"{databaseName}.mdf")}',
                    SIZE = 10MB, 
                    MAXSIZE = UNLIMITED, 
                    FILEGROWTH = 10MB
                )
                LOG ON 
                (
                    NAME = N'{databaseName}_log', 
                    FILENAME = N'{Path.Combine(logPath, $"{databaseName}_log.ldf")}',
                    SIZE = 5MB, 
                    MAXSIZE = 1GB, 
                    FILEGROWTH = 10%
                )
                COLLATE SQL_Latin1_General_CP1_CI_AS";

            using var connection = new SqlConnection(_masterConnectionString);
            await connection.OpenAsync(cancellationToken);

            using var command = new SqlCommand(sql, connection);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        private async Task<(string username, string password)> CreateDatabaseUserAsync(string databaseName, string tenantCode, CancellationToken cancellationToken)
        {
            var username = $"usr_{tenantCode}_{Guid.NewGuid():N}".Substring(0, 30);
            var password = GenerateSecurePassword();

            var sql = $@"
                -- Create login
                CREATE LOGIN [{username}] WITH PASSWORD = '{password}',
                DEFAULT_DATABASE = [{databaseName}],
                CHECK_EXPIRATION = OFF,
                CHECK_POLICY = OFF;

                -- Create user in the database
                USE [{databaseName}];
                CREATE USER [{username}] FOR LOGIN [{username}];
                ALTER USER [{username}] WITH DEFAULT_SCHEMA = [dbo];
                
                -- Grant permissions
                ALTER ROLE [db_owner] ADD MEMBER [{username}];
                
                USE [master];";

            using var connection = new SqlConnection(_masterConnectionString);
            await connection.OpenAsync(cancellationToken);

            using var command = new SqlCommand(sql, connection);
            await command.ExecuteNonQueryAsync(cancellationToken);

            return (username, password);
        }

        private async Task CloseAllConnectionsAsync(string databaseName, CancellationToken cancellationToken)
        {
            var sql = $@"
                ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                ALTER DATABASE [{databaseName}] SET MULTI_USER;";

            using var connection = new SqlConnection(_masterConnectionString);
            await connection.OpenAsync(cancellationToken);

            using var command = new SqlCommand(sql, connection);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        private async Task DropDatabaseAsync(string databaseName, CancellationToken cancellationToken)
        {
            var sql = $"DROP DATABASE IF EXISTS [{databaseName}]";

            using var connection = new SqlConnection(_masterConnectionString);
            await connection.OpenAsync(cancellationToken);

            using var command = new SqlCommand(sql, connection);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        private async Task DropLoginAsync(string username, CancellationToken cancellationToken)
        {
            var sql = $"DROP LOGIN IF EXISTS [{username}]";

            using var connection = new SqlConnection(_masterConnectionString);
            await connection.OpenAsync(cancellationToken);

            using var command = new SqlCommand(sql, connection);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        private async Task RollbackDatabaseCreationAsync(string databaseName, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogWarning("Rolling back database creation for {DatabaseName}", databaseName);

                if (await DatabaseExistsAsync(databaseName, cancellationToken))
                {
                    await DropDatabaseAsync(databaseName, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to rollback database creation for {DatabaseName}", databaseName);
            }
        }

        private string BuildTenantConnectionString(string databaseName, string username, string password)
        {
            var server = _configuration["DatabaseSettings:Server"] ?? "localhost";
            var encrypt = _configuration["DatabaseSettings:Encrypt"] ?? "true";
            var trustServerCertificate = _configuration["DatabaseSettings:TrustServerCertificate"] ?? "false";

            return $"Server={server};Database={databaseName};User Id={username};Password={password};" +
                   $"MultipleActiveResultSets=true;Encrypt={encrypt};TrustServerCertificate={trustServerCertificate};" +
                   $"Application Name=TenantApp_{databaseName};Connection Timeout=30;";
        }

        private string ExtractUsernameFromConnectionString(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            return builder.UserID;
        }

        private string GenerateSecurePassword()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789!@#$%^&*";
            var random = new Random();
            var password = new string(Enumerable.Repeat(chars, 16)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return password;
        }
        private string EncryptPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return password;

            // TODO: Production'da Azure Key Vault veya benzeri bir çözüm kullanın
            // Bu basit bir örnek
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(bytes);
        }

        private string EncryptConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                return connectionString;

            // TODO: Production'da Azure Key Vault veya benzeri bir çözüm kullanın
            // Bu basit bir örnek
            var bytes = System.Text.Encoding.UTF8.GetBytes(connectionString);
            return Convert.ToBase64String(bytes);
        }

        private string DecryptPassword(string encryptedPassword)
        {
            if (string.IsNullOrEmpty(encryptedPassword))
                return encryptedPassword;

            // TODO: Production'da Azure Key Vault veya benzeri bir çözüm kullanın
            var bytes = Convert.FromBase64String(encryptedPassword);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        private string DecryptConnectionString(string encryptedConnectionString)
        {
            if (string.IsNullOrEmpty(encryptedConnectionString))
                return encryptedConnectionString;

            // TODO: Production'da Azure Key Vault veya benzeri bir çözüm kullanın
            var bytes = Convert.FromBase64String(encryptedConnectionString);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
    } 
 
}
