using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Interfaces.Services
{
    public interface INotificationService
    {
        Task SendAsync(NotificationRequest notification, CancellationToken cancellationToken = default);
        Task SendToUserAsync(int userId, string title, string message, NotificationType type, CancellationToken cancellationToken = default);
        Task SendToRoleAsync(string role, string title, string message, NotificationType type, CancellationToken cancellationToken = default);
        Task SendToTenantAsync(int tenantId, string title, string message, NotificationType type, CancellationToken cancellationToken = default);
    }

    public class NotificationRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public Dictionary<string, object> Data { get; set; } = new();
        public List<int> UserIds { get; set; } = new();
        public List<string> Roles { get; set; } = new();
    }

    public enum NotificationType
    {
        Information,
        Success,
        Warning,
        Error
    }
}
