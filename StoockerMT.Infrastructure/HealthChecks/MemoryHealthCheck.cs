using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Infrastructure.HealthChecks
{
    public class MemoryHealthCheck : IHealthCheck
    {
        private readonly long _maximumAllocatedMegabytes;

        public MemoryHealthCheck(long maximumAllocatedMegabytes)
        {
            _maximumAllocatedMegabytes = maximumAllocatedMegabytes;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var allocatedMB = GC.GetTotalMemory(false) / 1024 / 1024;
            var gen0Collections = GC.CollectionCount(0);
            var gen1Collections = GC.CollectionCount(1);
            var gen2Collections = GC.CollectionCount(2);

            var data = new Dictionary<string, object>
            {
                { "AllocatedMB", allocatedMB },
                { "Gen0Collections", gen0Collections },
                { "Gen1Collections", gen1Collections },
                { "Gen2Collections", gen2Collections },
                { "TotalProcessorTime", Process.GetCurrentProcess().TotalProcessorTime }
            };

            var status = allocatedMB <= _maximumAllocatedMegabytes
                ? HealthCheckResult.Healthy($"Memory usage is within limits ({allocatedMB}MB)", data)
                : HealthCheckResult.Degraded($"Memory usage is high ({allocatedMB}MB)", null, data);

            return Task.FromResult(status);
        }
    }
}
