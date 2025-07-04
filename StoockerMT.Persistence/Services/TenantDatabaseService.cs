using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StoockerMT.Application.Common.Interfaces;
using StoockerMT.Domain.Entities.TenantDb;
using StoockerMT.Persistence.Contexts;

namespace StoockerMT.Persistence.Services
{
    public class TenantDatabaseService:ITenantDatabaseService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TenantDatabaseService> _logger;
        private readonly IDbContextFactory<TenantDbContext> _tenantDbContextFactory;

        public TenantDatabaseService(
            IConfiguration configuration,
            ILogger<TenantDatabaseService> logger,
            IDbContextFactory<TenantDbContext> tenantDbContextFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _tenantDbContextFactory = tenantDbContextFactory;
        }

        public async Task<bool> CreateTenantDatabaseAsync(string tenantCode)
        {
            var masterConnectionString = _configuration.GetConnectionString("MasterConnection");
            var databaseName = $"TenantDB_{tenantCode}";

            try
            {
                using (var connection = new SqlConnection(masterConnectionString))
                {
                    await connection.OpenAsync();

                    // Check if database already exists
                    var checkDbCommand = new SqlCommand(
                        $"SELECT database_id FROM sys.databases WHERE name = '{databaseName}'",
                        connection);
                    var databaseId = await checkDbCommand.ExecuteScalarAsync();

                    if (databaseId != null)
                    {
                        _logger.LogWarning("Database {DatabaseName} already exists", databaseName);
                        return false;
                    }

                    // Create database
                    var createDbCommand = new SqlCommand($"CREATE DATABASE [{databaseName}]", connection);
                    await createDbCommand.ExecuteNonQueryAsync();

                    _logger.LogInformation("Tenant database created successfully: {DatabaseName}", databaseName);
                }

                // Create connection string for the new database
                var builder = new SqlConnectionStringBuilder(masterConnectionString)
                {
                    InitialCatalog = databaseName
                };
                var tenantConnectionString = builder.ToString();

                // Run migrations on the new database
                var migrationResult = await MigrateTenantDatabaseAsync(tenantConnectionString);
                if (!migrationResult)
                {
                    _logger.LogError("Failed to migrate tenant database: {DatabaseName}", databaseName);
                    // Rollback - delete the created database
                    await DeleteTenantDatabaseAsync(databaseName);
                    return false;
                }

                // Seed initial data
                var seedResult = await SeedTenantDataAsync(tenantConnectionString);
                if (!seedResult)
                {
                    _logger.LogWarning("Failed to seed tenant data for database: {DatabaseName}", databaseName);
                    // Continue anyway - seeding might not be critical
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tenant database: {DatabaseName}", databaseName);
                return false;
            }
        }

        public async Task<bool> MigrateTenantDatabaseAsync(string connectionString)
        {
            try
            {
                // Create a new DbContext with the tenant connection string
                var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
                optionsBuilder.UseSqlServer(connectionString);

                using (var context = new TenantDbContext(optionsBuilder.Options))
                {
                    // Apply pending migrations
                    var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                    if (pendingMigrations.Any())
                    {
                        _logger.LogInformation("Applying {Count} pending migrations", pendingMigrations.Count());
                        await context.Database.MigrateAsync();
                        _logger.LogInformation("Migrations applied successfully");
                    }
                    else
                    {
                        _logger.LogInformation("No pending migrations to apply");
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error migrating tenant database");
                return false;
            }
        }

        public async Task<bool> SeedTenantDataAsync(string connectionString)
        {
            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
                optionsBuilder.UseSqlServer(connectionString);

                using (var context = new TenantDbContext(optionsBuilder.Options))
                {
                    // Check if data already exists
                    var hasData = await context.Set<Customer>().AnyAsync(); // Replace YourEntity with actual entity
                    if (hasData)
                    {
                        _logger.LogInformation("Database already contains data, skipping seeding");
                        return true;
                    }
 

                    await context.SaveChangesAsync();
                    _logger.LogInformation("Tenant data seeded successfully");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding tenant data");
                return false;
            }
        }

        public async Task<bool> DeleteTenantDatabaseAsync(string databaseName)
        {
            var masterConnectionString = _configuration.GetConnectionString("MasterConnection");

            try
            {
                using (var connection = new SqlConnection(masterConnectionString))
                {
                    await connection.OpenAsync();

                    // Check if database exists
                    var checkDbCommand = new SqlCommand(
                        $"SELECT database_id FROM sys.databases WHERE name = '{databaseName}'",
                        connection);
                    var databaseId = await checkDbCommand.ExecuteScalarAsync();

                    if (databaseId == null)
                    {
                        _logger.LogWarning("Database {DatabaseName} does not exist", databaseName);
                        return true; // Already deleted
                    }

                    // Set database to single user mode to close existing connections
                    try
                    {
                        var setSingleUserCommand = new SqlCommand(
                            $"ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE",
                            connection);
                        await setSingleUserCommand.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Could not set database to single user mode, attempting to drop anyway");
                    }

                    // Drop the database
                    var dropCommand = new SqlCommand($"DROP DATABASE [{databaseName}]", connection);
                    await dropCommand.ExecuteNonQueryAsync();

                    _logger.LogInformation("Tenant database deleted successfully: {DatabaseName}", databaseName);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tenant database: {DatabaseName}", databaseName);
                return false;
            }
        }
    }
}
