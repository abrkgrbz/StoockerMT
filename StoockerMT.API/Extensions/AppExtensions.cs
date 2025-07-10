using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StoockerMT.API.Middleware;

namespace StoockerMT.API.Extensions
{
    public static class AppExtensions
    {
        public static void UseHealthChecksExtension(this IApplicationBuilder app)
        {
            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status200OK,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                }
            });
        }

        public static void TenantMiddlewareExtensions(this IApplicationBuilder app)
        {
            app.UseMiddleware<TenantMiddleware>();
        }
    }
}
