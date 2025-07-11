using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Infrastructure.HealthChecks
{
    public class DiskSpaceHealthCheck : IHealthCheck
    {
        private readonly long _minimumFreeMegabytes;
        private readonly ILogger<DiskSpaceHealthCheck> _logger;

        public DiskSpaceHealthCheck(long minimumFreeMegabytes, ILogger<DiskSpaceHealthCheck> logger)
        {
            _minimumFreeMegabytes = minimumFreeMegabytes;
            _logger = logger;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var drives = DriveInfo.GetDrives()
                    .Where(d => d.IsReady && d.DriveType == DriveType.Fixed)
                    .ToList();

                var data = new Dictionary<string, object>();
                var unhealthyDrives = new List<string>();

                foreach (var drive in drives)
                {
                    var freeSpaceMB = drive.AvailableFreeSpace / 1024 / 1024;
                    var totalSpaceMB = drive.TotalSize / 1024 / 1024;
                    var usedPercentage = 100 - (freeSpaceMB * 100.0 / totalSpaceMB);

                    data[$"{drive.Name}_FreeSpaceMB"] = freeSpaceMB;
                    data[$"{drive.Name}_UsedPercentage"] = $"{usedPercentage:F2}%";

                    if (freeSpaceMB < _minimumFreeMegabytes)
                    {
                        unhealthyDrives.Add($"{drive.Name} ({freeSpaceMB}MB free)");
                    }
                }

                if (unhealthyDrives.Any())
                {
                    return Task.FromResult(HealthCheckResult.Unhealthy(
                        $"Insufficient disk space on: {string.Join(", ", unhealthyDrives)}",
                        null,
                        data));
                }

                return Task.FromResult(HealthCheckResult.Healthy("Sufficient disk space available", data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Disk space health check failed");
                return Task.FromResult(HealthCheckResult.Unhealthy("Unable to check disk space", ex));
            }
        }
    }

}
