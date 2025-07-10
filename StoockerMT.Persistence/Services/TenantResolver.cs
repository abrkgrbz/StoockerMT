using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StoockerMT.Application.Common.Interfaces.Services;
using StoockerMT.Domain.Entities.MasterDb;
using StoockerMT.Domain.Enums;
using StoockerMT.Domain.Repositories.UnitOfWork;
using StoockerMT.Domain.ValueObjects;

namespace StoockerMT.Persistence.Services
{
    public class TenantResolver : ITenantResolver
    {
        private readonly IMasterDbUnitOfWork _masterDbUnitOfWork;
        private readonly IMemoryCache _cache;
        private readonly ILogger<TenantResolver> _logger;
        private const string TENANT_CACHE_KEY = "tenant_cache_";
        private const int CACHE_DURATION_MINUTES = 30;

        public TenantResolver(
            IMasterDbUnitOfWork masterDbUnitOfWork,
            IMemoryCache cache,
            ILogger<TenantResolver> logger)
        {
            _masterDbUnitOfWork = masterDbUnitOfWork;
            _cache = cache;
            _logger = logger;
        }

        public async Task<Tenant> ResolveByIdAsync(int tenantId)
        {
            var cacheKey = $"{TENANT_CACHE_KEY}id_{tenantId}";

            if (_cache.TryGetValue<Tenant>(cacheKey, out var cachedTenant))
            {
                _logger.LogDebug("Tenant {TenantId} found in cache", tenantId);
                return cachedTenant;
            }

            var tenant = await _masterDbUnitOfWork.Tenants.GetByIdAsync(tenantId);

            if (tenant != null && tenant.Status == TenantStatus.Active)
            {
                CacheTenant(tenant);
                return tenant;
            }

            _logger.LogWarning("Tenant {TenantId} not found or inactive", tenantId);
            return null;
        }

        public async Task<Tenant> ResolveByCodeAsync(string tenantCode)
        {
            if (string.IsNullOrWhiteSpace(tenantCode))
                return null;

            var cacheKey = $"{TENANT_CACHE_KEY}code_{tenantCode.ToLowerInvariant()}";

            if (_cache.TryGetValue<Tenant>(cacheKey, out var cachedTenant))
            {
                _logger.LogDebug("Tenant {TenantCode} found in cache", tenantCode);
                return cachedTenant;
            }

            var tenantCodeValue = new TenantCode(tenantCode);
            var tenant = await _masterDbUnitOfWork.Tenants.GetByCodeAsync(tenantCodeValue);

            if (tenant != null && tenant.Status == TenantStatus.Active)
            {
                CacheTenant(tenant);
                return tenant;
            }

            _logger.LogWarning("Tenant {TenantCode} not found or inactive", tenantCode);
            return null;
        }

        public async Task<Tenant> ResolveByDomainAsync(string domain)
        {
            if (string.IsNullOrWhiteSpace(domain))
                return null;

            // Normalize domain
            domain = domain.ToLowerInvariant()
                .Replace("https://", "")
                .Replace("http://", "")
                .Replace("www.", "")
                .Split('/')[0];

            var cacheKey = $"{TENANT_CACHE_KEY}domain_{domain}";

            if (_cache.TryGetValue<Tenant>(cacheKey, out var cachedTenant))
            {
                _logger.LogDebug("Tenant for domain {Domain} found in cache", domain);
                return cachedTenant;
            }

            // Extract subdomain if exists (e.g., tenant1.stoockermt.com -> tenant1)
            var parts = domain.Split('.');
            if (parts.Length > 2)
            {
                var subdomain = parts[0];
                var tenant = await ResolveByCodeAsync(subdomain);
                if (tenant != null)
                {
                    _cache.Set(cacheKey, tenant, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));
                    return tenant;
                }
            }

            // Try to find by custom domain
            var tenants = await _masterDbUnitOfWork.Tenants.GetActiveTenantsAsync();
            var matchingTenant = tenants.FirstOrDefault(t =>
                t.Settings?.GetModuleSetting("system", "customDomain", "")?.ToString() == domain);

            if (matchingTenant != null)
            {
                CacheTenant(matchingTenant);
                _cache.Set(cacheKey, matchingTenant, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));
                return matchingTenant;
            }

            _logger.LogWarning("No tenant found for domain {Domain}", domain);
            return null;
        }

        public async Task<Tenant> ResolveByHeaderAsync(string headerValue)
        {
            if (string.IsNullOrWhiteSpace(headerValue))
                return null;

            // Header format: "tenant:{code}" or "id:{id}"
            var parts = headerValue.Split(':');
            if (parts.Length != 2)
                return null;

            var type = parts[0].ToLowerInvariant();
            var value = parts[1];

            return type switch
            {
                "tenant" => await ResolveByCodeAsync(value),
                "id" when int.TryParse(value, out var id) => await ResolveByIdAsync(id),
                _ => null
            };
        }

        public async Task<Tenant> ResolveByUserAsync(string userEmail)
        {
            if (string.IsNullOrWhiteSpace(userEmail))
                return null;

            var cacheKey = $"{TENANT_CACHE_KEY}user_{userEmail.ToLowerInvariant()}";

            if (_cache.TryGetValue<Tenant>(cacheKey, out var cachedTenant))
            {
                _logger.LogDebug("Tenant for user {UserEmail} found in cache", userEmail);
                return cachedTenant;
            }

            // Find all tenants with this user
            var tenants = await _masterDbUnitOfWork.Tenants.GetActiveTenantsAsync();

            foreach (var tenant in tenants)
            {
                var users = await _masterDbUnitOfWork.TenantUsers.GetByTenantAsync(tenant.Id);
                if (users.Any(u => u.Email.Value.Equals(userEmail, StringComparison.OrdinalIgnoreCase) && u.IsActive))
                {
                    CacheTenant(tenant);
                    _cache.Set(cacheKey, tenant, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));
                    return tenant;
                }
            }

            _logger.LogWarning("No tenant found for user {UserEmail}", userEmail);
            return null;
        }

        public async Task<Tenant> ResolveAsync(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                return null;

            // Try different resolution strategies

            // 1. Try as ID
            if (int.TryParse(identifier, out var id))
            {
                var tenantById = await ResolveByIdAsync(id);
                if (tenantById != null)
                    return tenantById;
            }

            // 2. Try as code
            var tenantByCode = await ResolveByCodeAsync(identifier);
            if (tenantByCode != null)
                return tenantByCode;

            // 3. Try as domain
            if (identifier.Contains('.'))
            {
                var tenantByDomain = await ResolveByDomainAsync(identifier);
                if (tenantByDomain != null)
                    return tenantByDomain;
            }

            // 4. Try as email
            if (identifier.Contains('@'))
            {
                var tenantByUser = await ResolveByUserAsync(identifier);
                if (tenantByUser != null)
                    return tenantByUser;
            }

            // 5. Try as header
            if (identifier.Contains(':'))
            {
                var tenantByHeader = await ResolveByHeaderAsync(identifier);
                if (tenantByHeader != null)
                    return tenantByHeader;
            }

            _logger.LogWarning("Could not resolve tenant with identifier: {Identifier}", identifier);
            return null;
        }

        private void CacheTenant(Tenant tenant)
        {
            var options = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(CACHE_DURATION_MINUTES))
                .SetPriority(CacheItemPriority.High);

            // Cache by multiple keys for quick lookup
            _cache.Set($"{TENANT_CACHE_KEY}id_{tenant.Id}", tenant, options);
            _cache.Set($"{TENANT_CACHE_KEY}code_{tenant.Code.Value.ToLowerInvariant()}", tenant, options);
        }
    }
}
