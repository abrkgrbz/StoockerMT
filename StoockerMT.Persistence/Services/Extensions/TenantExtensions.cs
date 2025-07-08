using System;
using StoockerMT.Domain.Entities.MasterDb;
using StoockerMT.Domain.Enums;
using StoockerMT.Domain.ValueObjects;

namespace StoockerMT.Domain.Extensions
{
    public static class TenantExtensions
    {
        // Database Info Management
        public static void SetDatabaseInfo(this Tenant tenant, DatabaseInfo databaseInfo)
        {
            if (databaseInfo == null)
                throw new ArgumentNullException(nameof(databaseInfo));

            tenant.SetDatabaseInfo(databaseInfo); 
        }

        public static void ClearDatabaseInfo(this Tenant tenant)
        {
           tenant.ClearDatabaseInfo();
        }

        // Settings Management
        public static void UpdateSettings(this Tenant tenant, TenantSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            tenant.UpdateSettings(settings);
        }
         
        public static void UpdateLastMigrationDate(this Tenant tenant, DateTime date)
        {
            if (tenant.DatabaseInfo == null)
                throw new InvalidOperationException("Cannot update migration date - no database info exists");
             
            var updatedDbInfo = tenant.DatabaseInfo.WithMaintenanceUpdate(lastMigrationDate: date);
            tenant.SetDatabaseInfo(updatedDbInfo);
        }

        public static void UpdateLastBackupDate(this Tenant tenant, DateTime date)
        {
            if (tenant.DatabaseInfo == null)
                throw new InvalidOperationException("Cannot update backup date - no database info exists");
             
            var updatedDbInfo = tenant.DatabaseInfo.WithMaintenanceUpdate(lastBackupDate: date);
            tenant.SetDatabaseInfo(updatedDbInfo);
        }

        public static void UpdateLastRestoreDate(this Tenant tenant, DateTime date)
        {
            if (tenant.DatabaseInfo == null)
                throw new InvalidOperationException("Cannot update restore date - no database info exists");

            // Create new DatabaseInfo instance with updated date
            var updatedDbInfo = tenant.DatabaseInfo.WithMaintenanceUpdate(lastRestoreDate: date);
            tenant.SetDatabaseInfo(updatedDbInfo);
        }

        public static void UpdateLastHealthCheckDate(this Tenant tenant, DateTime date)
        {
            if (tenant.DatabaseInfo == null)
                throw new InvalidOperationException("Cannot update health check date - no database info exists");

            // Create new DatabaseInfo instance with updated date
            var updatedDbInfo = tenant.DatabaseInfo.WithMaintenanceUpdate(lastHealthCheckDate: date);
            tenant.SetDatabaseInfo(updatedDbInfo);
        }

        public static void UpdateLastOptimizationDate(this Tenant tenant, DateTime date)
        {
            if (tenant.DatabaseInfo == null)
                throw new InvalidOperationException("Cannot update optimization date - no database info exists");

            // Create new DatabaseInfo instance with updated date
            var updatedDbInfo = tenant.DatabaseInfo.WithMaintenanceUpdate(lastOptimizationDate: date);
            tenant.SetDatabaseInfo(updatedDbInfo);
        }

        public static void UpdateSchemaVersion(this Tenant tenant, string version)
        {
            if (tenant.DatabaseInfo == null)
                throw new InvalidOperationException("Cannot update schema version - no database info exists");

            if (string.IsNullOrWhiteSpace(version))
                throw new ArgumentException("Schema version cannot be empty", nameof(version));

            // Create new DatabaseInfo instance with updated version
            var updatedDbInfo = tenant.DatabaseInfo.WithMaintenanceUpdate(schemaVersion: version);
            tenant.SetDatabaseInfo(updatedDbInfo);
        }

        // Combined maintenance update
        public static void UpdateMaintenanceInfo(this Tenant tenant,
            DateTime? migrationDate = null,
            DateTime? backupDate = null,
            DateTime? restoreDate = null,
            DateTime? healthCheckDate = null,
            DateTime? optimizationDate = null,
            string schemaVersion = null)
        {
            if (tenant.DatabaseInfo == null)
                throw new InvalidOperationException("Cannot update maintenance info - no database info exists");

            var updatedDbInfo = tenant.DatabaseInfo.WithMaintenanceUpdate(
                lastMigrationDate: migrationDate,
                lastBackupDate: backupDate,
                lastRestoreDate: restoreDate,
                lastHealthCheckDate: healthCheckDate,
                lastOptimizationDate: optimizationDate,
                schemaVersion: schemaVersion
            );

            tenant.SetDatabaseInfo(updatedDbInfo);
        }

        // Database Metadata Updates
        public static void UpdateDatabaseMetadata(this Tenant tenant,
            long? sizeInMB = null,
            int? maxSizeInMB = null,
            string collation = null,
            string recoveryModel = null,
            string compatibilityLevel = null)
        {
            if (tenant.DatabaseInfo == null)
                throw new InvalidOperationException("Cannot update database metadata - no database info exists");

            var updatedDbInfo = tenant.DatabaseInfo.WithMetadataUpdate(
                sizeInMB: sizeInMB,
                maxSizeInMB: maxSizeInMB,
                collation: collation,
                recoveryModel: recoveryModel,
                compatibilityLevel: compatibilityLevel
            );

            tenant.SetDatabaseInfo(updatedDbInfo);
        }

        // Performance Metrics Updates
        public static void UpdatePerformanceMetrics(this Tenant tenant,
            int? activeConnections = null,
            double? cpuUsagePercent = null,
            double? memoryUsageMB = null,
            double? diskIOReadMBPerSec = null,
            double? diskIOWriteMBPerSec = null)
        {
            if (tenant.DatabaseInfo == null)
                throw new InvalidOperationException("Cannot update performance metrics - no database info exists");

            var updatedDbInfo = tenant.DatabaseInfo.WithPerformanceMetrics(
                activeConnections: activeConnections,
                cpuUsagePercent: cpuUsagePercent,
                memoryUsageMB: memoryUsageMB,
                diskIOReadMBPerSec: diskIOReadMBPerSec,
                diskIOWriteMBPerSec: diskIOWriteMBPerSec
            );

            tenant.SetDatabaseInfo(updatedDbInfo);
        }

        // Settings Updates with specific methods
        public static void UpdateTimeZone(this Tenant tenant, string timeZone)
        {
            if (tenant.Settings == null)
                throw new InvalidOperationException("Cannot update timezone - no settings exist");

            var updatedSettings = tenant.Settings.WithTimeZone(timeZone);
            tenant.UpdateSettings(updatedSettings);
        }

        public static void UpdateDisplaySettings(this Tenant tenant,
            string themeColor = null,
            string logoUrl = null,
            int? itemsPerPage = null)
        {
            if (tenant.Settings == null)
                throw new InvalidOperationException("Cannot update display settings - no settings exist");

            var updatedSettings = tenant.Settings.WithDisplaySettings(
                themeColor: themeColor,
                logoUrl: logoUrl,
                itemsPerPage: itemsPerPage
            );

            tenant.UpdateSettings(updatedSettings);
        }

        public static void UpdateSecuritySettings(this Tenant tenant,
            int? passwordMinLength = null,
            bool? requireTwoFactor = null,
            int? sessionTimeoutMinutes = null,
            int? maxLoginAttempts = null)
        {
            if (tenant.Settings == null)
                throw new InvalidOperationException("Cannot update security settings - no settings exist");

            var updatedSettings = tenant.Settings.WithSecuritySettings(
                passwordMinLength: passwordMinLength,
                requireTwoFactor: requireTwoFactor,
                sessionTimeoutMinutes: sessionTimeoutMinutes,
                maxLoginAttempts: maxLoginAttempts
            );

            tenant.UpdateSettings(updatedSettings);
        }

        public static void SetModuleSetting(this Tenant tenant, string moduleCode, string key, object value)
        {
            if (tenant.Settings == null)
                throw new InvalidOperationException("Cannot set module setting - no settings exist");

            var updatedSettings = tenant.Settings.WithModuleSetting(moduleCode, key, value);
            tenant.UpdateSettings(updatedSettings);
        }

        // Query methods
        public static bool HasDatabase(this Tenant tenant)
        {
            return tenant.DatabaseInfo != null;
        }

        public static bool IsDatabaseHealthy(this Tenant tenant)
        {
            return tenant.DatabaseInfo?.IsHealthy() ?? false;
        }

        public static bool NeedsDatabaseBackup(this Tenant tenant, int backupIntervalDays = 1)
        {
            return tenant.DatabaseInfo?.NeedsBackup(backupIntervalDays) ?? false;
        }

        public static bool NeedsDatabaseOptimization(this Tenant tenant, int optimizationIntervalDays = 7)
        {
            return tenant.DatabaseInfo?.NeedsOptimization(optimizationIntervalDays) ?? false;
        }

        public static object GetModuleSetting(this Tenant tenant, string moduleCode, string key, object defaultValue = null)
        {
            return tenant.Settings?.GetModuleSetting(moduleCode, key, defaultValue) ?? defaultValue;
        }

        public static DateTime ConvertToTenantTime(this Tenant tenant, DateTime utcTime)
        {
            if (tenant.Settings == null)
                throw new InvalidOperationException("Cannot convert time - no settings exist");

            return tenant.Settings.ConvertToTenantTime(utcTime);
        }

        public static DateTime ConvertToUtc(this Tenant tenant, DateTime tenantTime)
        {
            if (tenant.Settings == null)
                throw new InvalidOperationException("Cannot convert time - no settings exist");

            return tenant.Settings.ConvertToUtc(tenantTime);
        }

        // Status management
        public static void Activate(this Tenant tenant)
        {
            if (tenant.Status == TenantStatus.Active)
                return;
            tenant.Activate();
        }

        public static void Deactivate(this Tenant tenant, string reason)
        {
            if (tenant.Status == TenantStatus.Inactive)
                return;
            tenant.Deactivate(reason);
        }

        public static void Suspend(this Tenant tenant, string reason)
        {
            if (tenant.Status == TenantStatus.Suspended)
                return;
            tenant.Suspend(reason);
        }

        // Validation methods
        public static bool IsActive(this Tenant tenant)
        {
            return tenant.Status == TenantStatus.Active;
        }

        public static bool CanAccessModule(this Tenant tenant, string moduleCode)
        {
            if (!tenant.IsActive())
                return false;

            // Check if tenant has active subscription for the module
            return tenant.ModuleSubscriptions?.Any(s =>
                s.Module.Code == moduleCode &&
                s.Status == SubscriptionStatus.Active &&
                s.SubscriptionPeriod.Contains(DateTime.UtcNow)) ?? false;
        }

        public static bool CanCreateUsers(this Tenant tenant, int maxUsers)
        {
            if (!tenant.IsActive())
                return false;

            var currentUserCount = tenant.Users?.Count(u => u.IsActive) ?? 0;
            return currentUserCount < maxUsers;
        }
    }
}