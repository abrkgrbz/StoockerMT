using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

namespace StoockerMT.Domain.ValueObjects
{
     
    public class DatabaseInfo : BaseValueObject
    { 
        public string DatabaseName { get; private set; }
        public string Server { get; private set; }
        public string Username { get; private set; }
        public string EncryptedPassword { get; private set; }
        public string EncryptedConnectionString { get; private set; }
         
        public int Port { get; private set; }
        public bool UseWindowsAuthentication { get; private set; }
        public bool Encrypt { get; private set; }
        public bool TrustServerCertificate { get; private set; }
        public int ConnectionTimeout { get; private set; }
        public int CommandTimeout { get; private set; }
        public string ApplicationName { get; private set; }
         
        public long? SizeInMB { get; private set; }
        public int? MaxSizeInMB { get; private set; }
        public string Collation { get; private set; }
        public string RecoveryModel { get; private set; }
        public string CompatibilityLevel { get; private set; }
         
        public DateTime CreatedDate { get; private set; }
        public DateTime? LastMigrationDate { get; private set; }
        public DateTime? LastBackupDate { get; private set; }
        public DateTime? LastRestoreDate { get; private set; }
        public DateTime? LastHealthCheckDate { get; private set; }
        public DateTime? LastOptimizationDate { get; private set; }
        public string SchemaVersion { get; private set; }
         
        public int? ActiveConnections { get; private set; }
        public double? CpuUsagePercent { get; private set; }
        public double? MemoryUsageMB { get; private set; }
        public double? DiskIOReadMBPerSec { get; private set; }
        public double? DiskIOWriteMBPerSec { get; private set; }
         
        private DatabaseInfo()
        { 
            DatabaseName = string.Empty;
            Server = string.Empty;
            Username = string.Empty;
            EncryptedPassword = string.Empty;
            EncryptedConnectionString = string.Empty;
            ApplicationName = string.Empty;
            Collation = "SQL_Latin1_General_CP1_CI_AS";
            RecoveryModel = "FULL";
            CompatibilityLevel = "150";
            SchemaVersion = "Initial";
            CreatedDate = DateTime.UtcNow;
        }
         
        private DatabaseInfo(
            string databaseName,
            string server,
            string username,
            string encryptedPassword,
            string encryptedConnectionString,
            int port,
            bool useWindowsAuthentication,
            bool encrypt,
            bool trustServerCertificate,
            int connectionTimeout,
            int commandTimeout,
            string applicationName,
            string collation,
            string recoveryModel,
            string compatibilityLevel,
            DateTime createdDate,
            string schemaVersion)
        {
            DatabaseName = databaseName;
            Server = server;
            Username = username;
            EncryptedPassword = encryptedPassword;
            EncryptedConnectionString = encryptedConnectionString;
            Port = port;
            UseWindowsAuthentication = useWindowsAuthentication;
            Encrypt = encrypt;
            TrustServerCertificate = trustServerCertificate;
            ConnectionTimeout = connectionTimeout;
            CommandTimeout = commandTimeout;
            ApplicationName = applicationName;
            Collation = collation;
            RecoveryModel = recoveryModel;
            CompatibilityLevel = compatibilityLevel;
            CreatedDate = createdDate;
            SchemaVersion = schemaVersion ?? "Initial";
        }

        // Factory method for creating new instances
        public static DatabaseInfo Create(
            string databaseName,
            string server,
            string username,
            string encryptedPassword,
            string encryptedConnectionString,
            int port = 1433,
            bool useWindowsAuthentication = false,
            bool encrypt = true,
            bool trustServerCertificate = false,
            int connectionTimeout = 30,
            int commandTimeout = 30,
            string applicationName = null,
            string collation = "SQL_Latin1_General_CP1_CI_AS",
            string recoveryModel = "FULL",
            string compatibilityLevel = "150")
        {
            // Validations
            if (string.IsNullOrWhiteSpace(databaseName))
                throw new ArgumentException("Database name cannot be empty", nameof(databaseName));

            if (string.IsNullOrWhiteSpace(server))
                throw new ArgumentException("Server cannot be empty", nameof(server));

            if (!useWindowsAuthentication)
            {
                if (string.IsNullOrWhiteSpace(username))
                    throw new ArgumentException("Username cannot be empty when not using Windows Authentication", nameof(username));

                if (string.IsNullOrWhiteSpace(encryptedPassword))
                    throw new ArgumentException("Password cannot be empty when not using Windows Authentication", nameof(encryptedPassword));
            }

            if (string.IsNullOrWhiteSpace(encryptedConnectionString))
                throw new ArgumentException("Connection string cannot be empty", nameof(encryptedConnectionString));

            if (port < 1 || port > 65535)
                throw new ArgumentOutOfRangeException(nameof(port), "Port must be between 1 and 65535");

            if (connectionTimeout < 0 || connectionTimeout > 300)
                throw new ArgumentOutOfRangeException(nameof(connectionTimeout), "Connection timeout must be between 0 and 300 seconds");

            if (commandTimeout < 0 || commandTimeout > 3600)
                throw new ArgumentOutOfRangeException(nameof(commandTimeout), "Command timeout must be between 0 and 3600 seconds");

            // Validate database name (SQL Server naming rules)
            if (!IsValidDatabaseName(databaseName))
                throw new ArgumentException($"Invalid database name: {databaseName}", nameof(databaseName));

            // Validate server name/IP
            if (!IsValidServerName(server))
                throw new ArgumentException($"Invalid server name: {server}", nameof(server));

            // Create instance using private constructor
            var dbInfo = new DatabaseInfo
            {
                DatabaseName = databaseName,
                Server = server,
                Username = username,
                EncryptedPassword = encryptedPassword,
                EncryptedConnectionString = encryptedConnectionString,
                Port = port,
                UseWindowsAuthentication = useWindowsAuthentication,
                Encrypt = encrypt,
                TrustServerCertificate = trustServerCertificate,
                ConnectionTimeout = connectionTimeout,
                CommandTimeout = commandTimeout,
                ApplicationName = applicationName ?? $"TenantApp_{databaseName}",
                Collation = collation,
                RecoveryModel = recoveryModel,
                CompatibilityLevel = compatibilityLevel,
                CreatedDate = DateTime.UtcNow,
                SchemaVersion = "Initial"
            };

            return dbInfo;
        }
         
        public static DatabaseInfo CreateFromConnectionString(string encryptedConnectionString, string encryptedPassword = null)
        {
            if (string.IsNullOrWhiteSpace(encryptedConnectionString))
                throw new ArgumentException("Connection string cannot be empty", nameof(encryptedConnectionString));
             
            var connectionString = DecryptConnectionString(encryptedConnectionString);
            var builder = new SqlConnectionStringBuilder(connectionString);

            var databaseName = builder.InitialCatalog;
            var server = builder.DataSource;
            var username = builder.UserID;
            var useWindowsAuth = builder.IntegratedSecurity;
            var encrypt = builder.Encrypt;
            var trustCert = builder.TrustServerCertificate;
            var timeout = builder.ConnectTimeout;
            var appName = builder.ApplicationName;

            // Extract port if specified in server (e.g., "server,1433")
            var port = 1433;
            if (server.Contains(","))
            {
                var parts = server.Split(',');
                server = parts[0];
                if (int.TryParse(parts[1], out var parsedPort))
                    port = parsedPort;
            }

            return Create(
                databaseName: databaseName,
                server: server,
                username: username,
                encryptedPassword: encryptedPassword ?? EncryptPassword(builder.Password),
                encryptedConnectionString: encryptedConnectionString,
                port: port,
                useWindowsAuthentication: useWindowsAuth,
                encrypt: encrypt,
                trustServerCertificate: trustCert,
                connectionTimeout: timeout,
                applicationName: appName
            );
        }
         
        public DatabaseInfo WithMaintenanceUpdate(
            DateTime? lastMigrationDate = null,
            DateTime? lastBackupDate = null,
            DateTime? lastRestoreDate = null,
            DateTime? lastHealthCheckDate = null,
            DateTime? lastOptimizationDate = null,
            string schemaVersion = null)
        {
            var copy = MemberwiseClone() as DatabaseInfo;

            copy.LastMigrationDate = lastMigrationDate ?? LastMigrationDate;
            copy.LastBackupDate = lastBackupDate ?? LastBackupDate;
            copy.LastRestoreDate = lastRestoreDate ?? LastRestoreDate;
            copy.LastHealthCheckDate = lastHealthCheckDate ?? LastHealthCheckDate;
            copy.LastOptimizationDate = lastOptimizationDate ?? LastOptimizationDate;
            copy.SchemaVersion = schemaVersion ?? SchemaVersion;

            return copy;
        }

        public DatabaseInfo WithMetadataUpdate(
            long? sizeInMB = null,
            int? maxSizeInMB = null,
            string collation = null,
            string recoveryModel = null,
            string compatibilityLevel = null)
        {
            var copy = MemberwiseClone() as DatabaseInfo;

            copy.SizeInMB = sizeInMB ?? SizeInMB;
            copy.MaxSizeInMB = maxSizeInMB ?? MaxSizeInMB;
            copy.Collation = collation ?? Collation;
            copy.RecoveryModel = recoveryModel ?? RecoveryModel;
            copy.CompatibilityLevel = compatibilityLevel ?? CompatibilityLevel;

            return copy;
        }

        public DatabaseInfo WithPerformanceMetrics(
            int? activeConnections = null,
            double? cpuUsagePercent = null,
            double? memoryUsageMB = null,
            double? diskIOReadMBPerSec = null,
            double? diskIOWriteMBPerSec = null)
        {
            var copy = MemberwiseClone() as DatabaseInfo;

            copy.ActiveConnections = activeConnections ?? ActiveConnections;
            copy.CpuUsagePercent = cpuUsagePercent ?? CpuUsagePercent;
            copy.MemoryUsageMB = memoryUsageMB ?? MemoryUsageMB;
            copy.DiskIOReadMBPerSec = diskIOReadMBPerSec ?? DiskIOReadMBPerSec;
            copy.DiskIOWriteMBPerSec = diskIOWriteMBPerSec ?? DiskIOWriteMBPerSec;

            return copy;
        }

        // Helper methods
        public string GetDecryptedConnectionString()
        {
            return DecryptConnectionString(EncryptedConnectionString);
        }

        public string GetDecryptedPassword()
        {
            return DecryptPassword(EncryptedPassword);
        }

        public string BuildConnectionString(bool includeDatabase = true)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = Port == 1433 ? Server : $"{Server},{Port}",
                InitialCatalog = includeDatabase ? DatabaseName : "master",
                IntegratedSecurity = UseWindowsAuthentication,
                Encrypt = Encrypt,
                TrustServerCertificate = TrustServerCertificate,
                ConnectTimeout = ConnectionTimeout,
                ApplicationName = ApplicationName,
                MultipleActiveResultSets = true
            };

            if (!UseWindowsAuthentication)
            {
                builder.UserID = Username;
                builder.Password = GetDecryptedPassword();
            }

            return builder.ConnectionString;
        }

        public bool IsHealthy()
        {
            // Consider database healthy if checked within last hour
            return LastHealthCheckDate.HasValue &&
                   (DateTime.UtcNow - LastHealthCheckDate.Value).TotalHours < 1;
        }

        public bool NeedsBackup(int backupIntervalDays = 1)
        {
            return !LastBackupDate.HasValue ||
                   (DateTime.UtcNow - LastBackupDate.Value).TotalDays >= backupIntervalDays;
        }

        public bool NeedsOptimization(int optimizationIntervalDays = 7)
        {
            return !LastOptimizationDate.HasValue ||
                   (DateTime.UtcNow - LastOptimizationDate.Value).TotalDays >= optimizationIntervalDays;
        }

        public double GetDatabaseAgeInDays()
        {
            return (DateTime.UtcNow - CreatedDate).TotalDays;
        }

        // Validation methods
        private static bool IsValidDatabaseName(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > 128)
                return false;

            var pattern = @"^[a-zA-Z_@#][a-zA-Z0-9_@$#]*$";
            return Regex.IsMatch(name, pattern);
        }

        private static bool IsValidServerName(string server)
        {
            if (string.IsNullOrWhiteSpace(server))
                return false;

            // Check if it's an IP address
            if (System.Net.IPAddress.TryParse(server, out _))
                return true;

            // Check if it's a valid hostname/FQDN
            var hostnamePattern = @"^([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])(\.([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]{0,61}[a-zA-Z0-9]))*$";
            return Regex.IsMatch(server, hostnamePattern);
        }

        // Encryption/Decryption methods
        private static string EncryptPassword(string password)
        {
            // TODO: Implement proper encryption using Azure Key Vault or similar
            if (string.IsNullOrEmpty(password)) return password;
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }

        private static string DecryptPassword(string encryptedPassword)
        {
            // TODO: Implement proper decryption using Azure Key Vault or similar
            if (string.IsNullOrEmpty(encryptedPassword)) return encryptedPassword;
            return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encryptedPassword));
        }

        private static string DecryptConnectionString(string encryptedConnectionString)
        {
            // TODO: Implement proper decryption using Azure Key Vault or similar
            if (string.IsNullOrEmpty(encryptedConnectionString)) return encryptedConnectionString;
            return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encryptedConnectionString));
        }

        // Value Object equality members
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return DatabaseName;
            yield return Server;
            yield return Username;
            yield return Port;
            yield return UseWindowsAuthentication;
            yield return ApplicationName;
        }

        public override string ToString()
        {
            return $"{Server}:{Port}/{DatabaseName} ({Username ?? "Windows Auth"})";
        }

         
    }
}