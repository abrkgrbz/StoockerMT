using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Identity.Configuration
{
    public class TenantResolutionConfig
    {
        public ResolutionStrategy Strategy { get; set; } = ResolutionStrategy.Header;
        public string HeaderName { get; set; } = "X-Tenant-ID";
        public bool RequireTenantForAllRequests { get; set; } = true;
        public string[] ExcludedPaths { get; set; } = new[]
        {
            "/health",
            "/swagger",
            "/api/auth/login",
            "/api/auth/register",
            "/api/tenants/register",
            "/.well-known"
        };
        public int CacheDurationMinutes { get; set; } = 30;
        public bool EnableSubdomainResolution { get; set; } = true;
        public string BaseUrl { get; set; } = "stoockermt.com";
    }

    public enum ResolutionStrategy
    {
        Header,
        Subdomain,
        Route,
        QueryString,
        Claim,
        Combined
    }
}
