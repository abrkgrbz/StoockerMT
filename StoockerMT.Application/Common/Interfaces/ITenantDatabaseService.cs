using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Interfaces
{
    public interface ITenantDatabaseService
    {
        /// <summary>
        /// Creates a new tenant database with the specified tenant code
        /// </summary>
        /// <param name="tenantCode">Unique tenant identifier</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> CreateTenantDatabaseAsync(string tenantCode);

        /// <summary>
        /// Applies pending migrations to a tenant database
        /// </summary>
        /// <param name="connectionString">Connection string to the tenant database</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> MigrateTenantDatabaseAsync(string connectionString);

        /// <summary>
        /// Seeds initial data into a tenant database
        /// </summary>
        /// <param name="connectionString">Connection string to the tenant database</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> SeedTenantDataAsync(string connectionString);

        /// <summary>
        /// Deletes a tenant database
        /// </summary>
        /// <param name="databaseName">Name of the database to delete</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> DeleteTenantDatabaseAsync(string databaseName);
    }
}
