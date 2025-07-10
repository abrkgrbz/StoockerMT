using Microsoft.Extensions.Diagnostics.HealthChecks;
using StoockerMT.Infrastructure.HealthChecks;

namespace StoockerMT.API.Extensions
{
    public static class ServiceExtension
    {
        public static void AddHealthChecksExtension(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck<MasterDatabaseHealthCheck>("master_db",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "db", "master" })
                .AddCheck<TenantDatabaseHealthCheck>("tenant_db",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "db", "tenant" });
        }
    }
}
