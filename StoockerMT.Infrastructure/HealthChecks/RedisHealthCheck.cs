using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Infrastructure.HealthChecks
{
    public class RedisHealthCheck : IHealthCheck
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<RedisHealthCheck> _logger;

        public RedisHealthCheck(IConnectionMultiplexer redis, ILogger<RedisHealthCheck> logger)
        {
            _redis = redis;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var database = _redis.GetDatabase();
                await database.PingAsync();

                var server = _redis.GetServer(_redis.GetEndPoints().First());
                var info = await server.InfoAsync();

                var data = new Dictionary<string, object>
                {
                    { "Connected", _redis.IsConnected },
                    { "Endpoints", string.Join(", ", _redis.GetEndPoints().Select(e => e.ToString())) },
                    { "ConnectedClients", info.FirstOrDefault(i => i.Key == "Clients")?.FirstOrDefault(c => c.Key == "connected_clients").Value ?? "Unknown" }
                };

                return HealthCheckResult.Healthy("Redis is accessible", data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis health check failed");
                return HealthCheckResult.Unhealthy("Redis is not accessible", ex);
            }
        }
    }

}
