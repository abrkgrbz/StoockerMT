using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Constants
{
    public static class CacheKeys
    {
        public static string GetTenantKey(int tenantId) => $"tenant:{tenantId}";
        public static string GetUserKey(int userId) => $"user:{userId}";
        public static string GetModuleKey(int moduleId) => $"module:{moduleId}";
        public static string GetTenantModulesKey(int tenantId) => $"tenant:{tenantId}:modules";
        public static string GetUserPermissionsKey(int userId) => $"user:{userId}:permissions";
        public static string GetProductKey(int productId) => $"product:{productId}";
        public static string GetCustomerKey(int customerId) => $"customer:{customerId}";
    }

    public static class CacheExpiration
    {
        public static readonly TimeSpan Short = TimeSpan.FromMinutes(5);
        public static readonly TimeSpan Medium = TimeSpan.FromMinutes(30);
        public static readonly TimeSpan Long = TimeSpan.FromHours(4);
        public static readonly TimeSpan Daily = TimeSpan.FromDays(1);
    }
}
