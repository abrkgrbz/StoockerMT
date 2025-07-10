using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using StoockerMT.Application.Common.Interfaces.Services;
using StoockerMT.Application.Common.Interfaces;
using StoockerMT.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Infrastructure.HealthChecks
{
    public class TenantDatabaseHealthCheck : IHealthCheck
    {
        private readonly IDbContextFactory<TenantDbContext> _contextFactory;
        private readonly ICurrentTenantService _currentTenantService;
        private readonly IResilientDatabaseService _resilientService;
        private readonly ILogger<TenantDatabaseHealthCheck> _logger;

        public TenantDatabaseHealthCheck(
            IDbContextFactory<TenantDbContext> contextFactory,
            ICurrentTenantService currentTenantService,
            IResilientDatabaseService resilientService,
            ILogger<TenantDatabaseHealthCheck> logger)
        {
            _contextFactory = contextFactory;
            _currentTenantService = currentTenantService;
            _resilientService = resilientService;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            if (!_currentTenantService.HasTenant())
            {
                return HealthCheckResult.Healthy("No tenant context - skipping tenant database health check");
            }

            try
            {
                using var dbContext = await _contextFactory.CreateDbContextAsync(cancellationToken);

                var startTime = DateTime.UtcNow;
                var isHealthy = await _resilientService.IsDatabaseHealthyAsync(dbContext, cancellationToken);
                var responseTime = DateTime.UtcNow - startTime;

                if (!isHealthy)
                {
                    return HealthCheckResult.Unhealthy(
                        $"Tenant database is not healthy for tenant: {_currentTenantService.TenantCode}",
                        data: new Dictionary<string, object>
                        {
                            ["ResponseTime"] = responseTime.TotalMilliseconds,
                            ["TenantCode"] = _currentTenantService.TenantCode,
                            ["TenantId"] = _currentTenantService.TenantId ?? 0
                        });
                }

                // Additional tenant-specific checks
                var customerCount = await dbContext.Customers.CountAsync(cancellationToken);
                var orderCount = await dbContext.Orders.CountAsync(cancellationToken);

                var data = new Dictionary<string, object>
                {
                    ["ResponseTime"] = responseTime.TotalMilliseconds,
                    ["TenantCode"] = _currentTenantService.TenantCode,
                    ["TenantId"] = _currentTenantService.TenantId ?? 0,
                    ["CustomerCount"] = customerCount,
                    ["OrderCount"] = orderCount
                };

                if (responseTime.TotalMilliseconds > 1000)
                {
                    return HealthCheckResult.Degraded(
                        $"Tenant database response time is slow: {responseTime.TotalMilliseconds}ms",
                        data: data);
                }

                return HealthCheckResult.Healthy(
                    $"Tenant database is healthy for tenant: {_currentTenantService.TenantCode}",
                    data: data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tenant database health check failed for tenant: {TenantCode}",
                    _currentTenantService.TenantCode);

                return HealthCheckResult.Unhealthy(
                    $"Tenant database health check failed for tenant: {_currentTenantService.TenantCode}",
                    exception: ex);
            }
        }
    }
}
