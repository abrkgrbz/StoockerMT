using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoockerMT.Application.Common.Interfaces.Services;
using StoockerMT.Persistence.Contexts;

namespace StoockerMT.Infrastructure.HealthChecks
{
    public class MasterDatabaseHealthCheck : IHealthCheck
    {
        private readonly MasterDbContext _context;
        private readonly IResilientDatabaseService _resilientService;
        private readonly ILogger<MasterDatabaseHealthCheck> _logger;

        public MasterDatabaseHealthCheck(
            MasterDbContext context,
            IResilientDatabaseService resilientService,
            ILogger<MasterDatabaseHealthCheck> logger)
        {
            _context = context;
            _resilientService = resilientService;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var startTime = DateTime.UtcNow;
                var isHealthy = await _resilientService.IsDatabaseHealthyAsync(_context, cancellationToken);
                var responseTime = DateTime.UtcNow - startTime;

                if (!isHealthy)
                {
                    return HealthCheckResult.Unhealthy(
                        "Master database is not healthy",
                        data: new Dictionary<string, object>
                        {
                            ["ResponseTime"] = responseTime.TotalMilliseconds,
                            ["Database"] = "Master"
                        });
                }

                // Additional checks
                var tenantCount = await _context.Tenants.CountAsync(cancellationToken);
                var activeConnections = await GetActiveConnectionsAsync(cancellationToken);

                var data = new Dictionary<string, object>
                {
                    ["ResponseTime"] = responseTime.TotalMilliseconds,
                    ["Database"] = "Master",
                    ["TenantCount"] = tenantCount,
                    ["ActiveConnections"] = activeConnections
                };

                if (responseTime.TotalMilliseconds > 1000)
                {
                    return HealthCheckResult.Degraded(
                        $"Master database response time is slow: {responseTime.TotalMilliseconds}ms",
                        data: data);
                }

                return HealthCheckResult.Healthy(
                    "Master database is healthy",
                    data: data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Master database health check failed");
                return HealthCheckResult.Unhealthy(
                    "Master database health check failed",
                    exception: ex);
            }
        }

        private async Task<int> GetActiveConnectionsAsync(CancellationToken cancellationToken)
        {
            try
            {
                var connectionString = _context.Database.GetConnectionString();
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync(cancellationToken);

                using var command = new SqlCommand(@"
                    SELECT COUNT(*) 
                    FROM sys.dm_exec_connections 
                    WHERE database_id = DB_ID()", connection);

                var result = await command.ExecuteScalarAsync(cancellationToken);
                return (int)result;
            }
            catch
            {
                return -1;
            }
        }
    }
}
