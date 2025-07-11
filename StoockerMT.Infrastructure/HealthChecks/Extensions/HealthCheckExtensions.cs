using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Infrastructure.HealthChecks.Extensions
{
    public static class HealthCheckExtensions
    {
        public static IHealthChecksBuilder AddCustomHealthChecks(
            this IHealthChecksBuilder builder,
            IConfiguration configuration)
        {
            builder
                .AddTypeActivatedCheck<MasterDatabaseHealthCheck>(
                    "database",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "db", "sql", "master" })
                .AddTypeActivatedCheck<TenantDatabaseHealthCheck>(
                    "database",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "db", "sql", "master" })
                .AddTypeActivatedCheck<DiskSpaceHealthCheck>(
                    "disk-space",
                    failureStatus: HealthStatus.Degraded,
                    tags: new[] { "infrastructure" },
                    args: new object[] { 1024L }) // 1GB minimum
                .AddTypeActivatedCheck<MemoryHealthCheck>(
                    "memory",
                    failureStatus: HealthStatus.Degraded,
                    tags: new[] { "infrastructure" },
                    args: new object[] { 1024L }); // 1GB maximum

            // Add Redis health check if Redis is enabled
            if (configuration.GetValue<bool>("CacheSettings:UseRedis"))
            {
                builder.AddTypeActivatedCheck<RedisHealthCheck>(
                    "redis",
                    failureStatus: HealthStatus.Degraded,
                    tags: new[] { "cache", "redis" });
            }

            return builder;
        }
    }
}
