using Microsoft.Extensions.Caching.Memory;
using StoockerMT.Application.Common.Interfaces;

namespace StoockerMT.API.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TenantMiddleware> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;

        // Excluded paths as static to avoid recreation
        private static readonly string[] ExcludedPaths = new[]
        {
            "/health",
            "/swagger",
            "/api/auth/login",
            "/api/auth/register",
            "/api/tenants/register",
            "/.well-known",
            "/metrics",
            "/favicon.ico"
        };

        public TenantMiddleware(
            RequestDelegate next,
            ILogger<TenantMiddleware> logger,
            IMemoryCache memoryCache,
            IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _memoryCache = memoryCache;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        { 
            if (IsExcludedPath(context.Request.Path))
            {
                await _next(context);
                return;
            } 
            if (_configuration.GetValue<bool>("Development:SkipTenantValidation") &&
                context.Request.Host.Host == "localhost")
            {
                await _next(context);
                return;
            }

            try
            {
                // Get tenant info from request
                var tenantIdentifier = GetTenantIdentifier(context);

                if (string.IsNullOrEmpty(tenantIdentifier))
                {
                    _logger.LogWarning("No tenant identifier found for request: {Path}", context.Request.Path);
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = "Tenant identification required",
                        details = "Please provide tenant information via subdomain or X-Tenant-Id header"
                    });
                    return;
                }

                // Store tenant identifier in HttpContext items for later use
                context.Items["TenantIdentifier"] = tenantIdentifier;

                // Add tenant info to response headers
                context.Response.Headers["X-Tenant-Identifier"] = tenantIdentifier;

                // Add cache headers for tenant-specific responses
                if (!context.Response.Headers.ContainsKey("Cache-Control"))
                {
                    context.Response.Headers["Cache-Control"] = "private, max-age=300";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in tenant resolution middleware");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new { error = "Error resolving tenant" });
                return;
            }

            await _next(context);
        }

        private string? GetTenantIdentifier(HttpContext context)
        {
            // 1. Check X-Tenant-Id header
            var tenantHeader = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();
            if (!string.IsNullOrEmpty(tenantHeader))
            {
                return tenantHeader;
            }

            // 2. Check subdomain
            var host = context.Request.Host.Host;
            if (!string.IsNullOrEmpty(host) && !host.Equals("localhost", StringComparison.OrdinalIgnoreCase))
            {
                var subdomain = host.Split('.')[0];
                if (!string.IsNullOrEmpty(subdomain) && subdomain != "www")
                {
                    return subdomain;
                }
            }

            // 3. Check route parameter
            if (context.Request.RouteValues.TryGetValue("tenant", out var tenantRoute))
            {
                return tenantRoute?.ToString();
            }

            // 4. Check query string
            if (context.Request.Query.TryGetValue("tenant", out var tenantQuery))
            {
                return tenantQuery.FirstOrDefault();
            }

            return null;
        }

        private bool IsExcludedPath(PathString path)
        {
            var pathValue = path.Value?.ToLowerInvariant();
            if (string.IsNullOrEmpty(pathValue))
                return false;

            // Check static file extensions
            if (pathValue.EndsWith(".js") ||
                pathValue.EndsWith(".css") ||
                pathValue.EndsWith(".png") ||
                pathValue.EndsWith(".jpg") ||
                pathValue.EndsWith(".ico"))
            {
                return true;
            }

            // Check excluded paths
            foreach (var excludedPath in ExcludedPaths)
            {
                if (path.StartsWithSegments(excludedPath, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }

   
}