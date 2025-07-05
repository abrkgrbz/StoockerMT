using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Interfaces.Services
{
    public interface IAuditService
    {
        Task LogAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
        Task<IEnumerable<AuditLog>> GetLogsAsync(int? userId = null, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default);
    }

    public class AuditLog
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int? TenantId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public Dictionary<string, object> OldValues { get; set; } = new();
        public Dictionary<string, object> NewValues { get; set; } = new();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
    }
}
