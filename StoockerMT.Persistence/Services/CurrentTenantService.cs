using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StoockerMT.Application.Common.Interfaces;
using StoockerMT.Application.Common.Interfaces.Services;
using StoockerMT.Domain.Entities.MasterDb;
using StoockerMT.Domain.Repositories.UnitOfWork;
using StoockerMT.Domain.ValueObjects;

namespace StoockerMT.Persistence.Services
{
    public class CurrentTenantService : ICurrentTenantService, ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITenantResolver _tenantResolver;
        private readonly IMasterDbUnitOfWork _masterDbUnitOfWork;
        private readonly IEncryptionService _encryptionService;
        private readonly ILogger<CurrentTenantService> _logger;

        private Tenant _currentTenant;
        private TenantUser _currentUser;
        private string _connectionString;

        public CurrentTenantService(
            IHttpContextAccessor httpContextAccessor,
            ITenantResolver tenantResolver,
            IMasterDbUnitOfWork masterDbUnitOfWork,
            IEncryptionService encryptionService,
            ILogger<CurrentTenantService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _tenantResolver = tenantResolver;
            _masterDbUnitOfWork = masterDbUnitOfWork;
            _encryptionService = encryptionService;
            _logger = logger;
        }

        // Tenant Information
        public int? TenantId => _currentTenant?.Id;
        public string TenantCode => _currentTenant?.Code?.Value;
        public string TenantName => _currentTenant?.Name;

        public string ConnectionString
        {
            get
            {
                if (!string.IsNullOrEmpty(_connectionString))
                    return _connectionString;

                if (_currentTenant?.DatabaseInfo == null)
                    return null;

                try
                {
                    _connectionString = _encryptionService.Decrypt(_currentTenant.DatabaseInfo.EncryptedConnectionString);
                    return _connectionString;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to decrypt connection string for tenant {TenantId}", TenantId);
                    return null;
                }
            }
        }

        // User Information
        public int? UserId => _currentUser?.Id;
        public string UserEmail => _currentUser?.Email?.Value;
        public bool IsTenantAdmin => _currentUser?.IsTenantAdmin ?? false;

        // ICurrentUserService implementation
        string ICurrentUserService.UserId => UserId?.ToString();

        public string? UserName => throw new NotImplementedException();

        public string? Email => throw new NotImplementedException();

        bool ICurrentUserService.IsAuthenticated => throw new NotImplementedException();

        // Tenant Resolution Methods
        public async Task<bool> SetTenantAsync(string tenantIdentifier)
        {
            if (string.IsNullOrWhiteSpace(tenantIdentifier))
            {
                _logger.LogWarning("Empty tenant identifier provided");
                return false;
            }

            try
            {
                var tenant = await _tenantResolver.ResolveAsync(tenantIdentifier);
                if (tenant == null)
                {
                    _logger.LogWarning("Could not resolve tenant with identifier: {Identifier}", tenantIdentifier);
                    return false;
                }

                _currentTenant = tenant;
                _connectionString = null;  
                 
                await LoadCurrentUserAsync();

                _logger.LogInformation("Tenant {TenantCode} set successfully", tenant.Code.Value);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting tenant with identifier: {Identifier}", tenantIdentifier);
                return false;
            }
        }

        public async Task<bool> SetTenantByIdAsync(int tenantId)
        {
            var tenant = await _tenantResolver.ResolveByIdAsync(tenantId);
            if (tenant == null)
            {
                _logger.LogWarning("Tenant {TenantId} not found", tenantId);
                return false;
            }

            _currentTenant = tenant;
            _connectionString = null;
            await LoadCurrentUserAsync();

            return true;
        }

        public async Task<bool> SetTenantByCodeAsync(string tenantCode)
        {
            var tenant = await _tenantResolver.ResolveByCodeAsync(tenantCode);
            if (tenant == null)
            {
                _logger.LogWarning("Tenant {TenantCode} not found", tenantCode);
                return false;
            }

            _currentTenant = tenant;
            _connectionString = null;
            await LoadCurrentUserAsync();

            return true;
        }

        public async Task<bool> SetTenantByDomainAsync(string domain)
        {
            var tenant = await _tenantResolver.ResolveByDomainAsync(domain);
            if (tenant == null)
            {
                _logger.LogWarning("No tenant found for domain {Domain}", domain);
                return false;
            }

            _currentTenant = tenant;
            _connectionString = null;
            await LoadCurrentUserAsync();

            return true;
        }
         
        public bool HasTenant()
        {
            return _currentTenant != null;
        }

        public bool IsAuthenticated()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        }

        public async Task<bool> ValidateTenantAccessAsync()
        {
            if (!HasTenant() || !IsAuthenticated())
                return false;

            if (_currentUser == null)
                await LoadCurrentUserAsync();

            if (_currentUser == null)
                return false;
             
            return _currentUser.TenantId == _currentTenant.Id && _currentUser.IsActive;
        }
         
        private async Task LoadCurrentUserAsync()
        {
            if (!IsAuthenticated() || _currentTenant == null)
                return;

            var httpContext = _httpContextAccessor.HttpContext;
            var userEmailClaim = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(userEmailClaim))
            {
                _logger.LogWarning("Authenticated user has no email claim");
                return;
            }

            try
            {
                var email = new Email(userEmailClaim);
                _currentUser = await _masterDbUnitOfWork.TenantUsers.GetByEmailAsync(email, _currentTenant.Id);

                if (_currentUser == null)
                {
                    _logger.LogWarning("User {Email} not found in tenant {TenantId}", userEmailClaim, _currentTenant.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading current user");
            }
        }
         
        public async Task InitializeAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return;

            string tenantIdentifier = null;

            // 1. Check custom header
            if (httpContext.Request.Headers.TryGetValue("X-Tenant-ID", out var headerValue))
            {
                tenantIdentifier = headerValue.FirstOrDefault();
            }
            // 2. Check subdomain
            else if (httpContext.Request.Host.HasValue)
            {
                var host = httpContext.Request.Host.Value;
                var parts = host.Split('.');
                if (parts.Length > 2) // e.g., tenant1.stoockermt.com
                {
                    tenantIdentifier = parts[0];
                }
            }
            // 3. Check route data
            else if (httpContext.Request.RouteValues.TryGetValue("tenant", out var routeValue))
            {
                tenantIdentifier = routeValue?.ToString();
            }
            // 4. Check query string
            else if (httpContext.Request.Query.TryGetValue("tenant", out var queryValue))
            {
                tenantIdentifier = queryValue.FirstOrDefault();
            }
            // 5. Check user claim
            else if (IsAuthenticated())
            {
                var tenantClaim = httpContext.User.FindFirst("TenantId")?.Value;
                if (!string.IsNullOrEmpty(tenantClaim))
                {
                    tenantIdentifier = tenantClaim;
                }
                else
                {
                    // Try to resolve by user email
                    var emailClaim = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;
                    if (!string.IsNullOrEmpty(emailClaim))
                    {
                        var tenant = await _tenantResolver.ResolveByUserAsync(emailClaim);
                        if (tenant != null)
                        {
                            _currentTenant = tenant;
                            await LoadCurrentUserAsync();
                            return;
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(tenantIdentifier))
            {
                await SetTenantAsync(tenantIdentifier);
            }
        }
    }
}