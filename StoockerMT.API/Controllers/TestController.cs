using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoockerMT.Application.Common.Interfaces.Services;
using StoockerMT.Domain.Entities.MasterDb;
using StoockerMT.Domain.ValueObjects;
using StoockerMT.Identity.Attributes;
using StoockerMT.Persistence.Contexts;

namespace StoockerMT.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly MasterDbContext _masterContext;
        private readonly ITenantService _tenantService;
        private readonly ILogger<TestController> _logger;


        public TestController(MasterDbContext masterContext, ITenantService tenantService, ILogger<TestController> logger)
        {
            _masterContext = masterContext;
            _tenantService = tenantService;
            _logger = logger;
        }

        [HttpPost("create-test-tenant")] 
        public async Task<IActionResult> CreateTestTenant()
        { 
            try
            { 
                var tenantCode = await GenerateUniqueTenantCode();
                 
                var tenant = new Tenant(
                    name: "Test Company " + DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                    code: tenantCode,
                    createdBy: "System",
                    description: "Test tenant created via API"
                );
                 
                _masterContext.Tenants.Add(tenant);
                await _masterContext.SaveChangesAsync();

                _logger.LogInformation("Tenant created with ID: {TenantId}", tenant.Id);
                 
                var connectionString = await _tenantService.CreateTenantDatabaseAsync(
                    tenant.Id,
                    tenantCode.Value
                ); 
                return Ok(new
                {
                    Success = true,
                    Message = "Test tenant created successfully",
                    TenantId = tenant.Id,
                    TenantCode = tenantCode.Value,
                    DatabaseCreated = true,
                    // Production'da connection string'i döndürmeyin!
                    ConnectionString = connectionString
                });
            }
            catch (Exception ex)
            { 
                _logger.LogError(ex, "Failed to create test tenant");

                return BadRequest(new
                {
                    Success = false,
                    Message = "Failed to create test tenant",
                    Error = ex.Message
                });
            }
        }

        [HttpPost("create-tenant")]
        public async Task<IActionResult> CreateTenant([FromBody] CreateTenantRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using var transaction = await _masterContext.Database.BeginTransactionAsync();

            try
            {
                // 1. Validate request
                if (string.IsNullOrWhiteSpace(request.Name))
                    return BadRequest("Tenant name is required");

                if (string.IsNullOrWhiteSpace(request.Code))
                    return BadRequest("Tenant code is required");

                // 2. Check if tenant code already exists
                var tenantCode = new TenantCode(request.Code.ToUpperInvariant());
                var existingTenant = await _masterContext.Tenants
                    .FirstOrDefaultAsync(t => t.Code == tenantCode);

                if (existingTenant != null)
                    return Conflict($"Tenant with code '{request.Code}' already exists");

                // 3. Create tenant
                var tenant = new Tenant(
                    name: request.Name,
                    code: tenantCode,
                    createdBy: User.Identity?.Name ?? "System",
                    description: request.Description
                );

                // 4. Set limits if provided
                if (request.MaxUsers.HasValue || request.MaxStorageGB.HasValue || request.MaxModules.HasValue)
                {
                    tenant.UpdateLimits(
                        maxUsers: request.MaxUsers,
                        maxStorageBytes: (long?)(request.MaxStorageGB * 1024L * 1024L * 1024L),  
                        maxModules: request.MaxModules,
                        updatedBy: User.Identity?.Name ?? "System"
                    );
                }

                // 5. Add to database
                _masterContext.Tenants.Add(tenant);
                await _masterContext.SaveChangesAsync();

                // 6. Create tenant database
                var connectionString = await _tenantService.CreateTenantDatabaseAsync(
                    tenant.Id,
                    tenantCode.Value
                );

                // 7. Run initial migrations
                var migrationSuccess = await _tenantService.MigrateTenantDatabaseAsync(tenant.Id);
                if (!migrationSuccess)
                {
                    throw new Exception("Failed to run database migrations");
                }

                // 8. Commit transaction
                await transaction.CommitAsync();

                return CreatedAtAction(
                    nameof(GetTenant),
                    new { id = tenant.Id },
                    new TenantResponse
                    {
                        Id = tenant.Id,
                        Name = tenant.Name,
                        Code = tenant.Code.Value,
                        Status = tenant.Status.ToString(),
                        DatabaseCreated = true,
                        MigrationsApplied = migrationSuccess,
                        CreatedAt = tenant.CreatedAt
                    }
                );
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to create tenant");

                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while creating the tenant",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTenant(int id)
        {
            var tenant = await _masterContext.Tenants
                .Include(t => t.ModuleSubscriptions)
                .ThenInclude(s => s.Module)
                .Include(t => t.Users)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tenant == null)
                return NotFound();

            return Ok(new TenantDetailResponse
            {
                Id = tenant.Id,
                Name = tenant.Name,
                Code = tenant.Code.Value,
                Description = tenant.Description,
                Status = tenant.Status.ToString(),
                MaxUsers = tenant.MaxUsers,
                MaxStorageGB = tenant.MaxStorageBytes / (1024.0 * 1024.0 * 1024.0),
                MaxModules = tenant.MaxModules,
                ActiveUsers = tenant.GetActiveUserCount(),
                ActiveModules = tenant.GetActiveModuleCount(),
                HasDatabase = tenant.HasDatabase(),
                IsDatabaseHealthy = tenant.IsDatabaseHealthy(),
                CreatedAt = tenant.CreatedAt,
                CreatedBy = tenant.CreatedBy,
                UpdatedAt = tenant.UpdatedAt,
                UpdatedBy = tenant.UpdatedBy
            });
        }

        [HttpPost("{id}/run-migrations")]
        public async Task<IActionResult> RunMigrations(int id)
        {
            try
            {
                var success = await _tenantService.MigrateTenantDatabaseAsync(id);

                if (success)
                    return Ok(new { Success = true, Message = "Migrations completed successfully" });

                return BadRequest(new { Success = false, Message = "Failed to run migrations" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to run migrations for tenant {TenantId}", id);
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost("{id}/check-health")]
        public async Task<IActionResult> CheckDatabaseHealth(int id)
        {
            try
            {
                var isHealthy = await _tenantService.CheckTenantDatabaseHealthAsync(id);

                return Ok(new
                {
                    TenantId = id,
                    IsHealthy = isHealthy,
                    CheckedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check database health for tenant {TenantId}", id);
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        private async Task<TenantCode> GenerateUniqueTenantCode()
        {
            var random = new Random();
            TenantCode tenantCode;
            bool exists;

            do
            {
                var code = $"TEST{random.Next(1000, 9999)}";
                tenantCode = new TenantCode(code);

                exists = false;
            }
            while (exists);

            return tenantCode;
        }
    }

    // Request/Response DTOs
    public class CreateTenantRequest
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string? Description { get; set; }
        public int? MaxUsers { get; set; }
        public double? MaxStorageGB { get; set; }
        public int? MaxModules { get; set; }
    }

    public class TenantResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Status { get; set; }
        public bool DatabaseCreated { get; set; }
        public bool MigrationsApplied { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class TenantDetailResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; }
        public int MaxUsers { get; set; }
        public double MaxStorageGB { get; set; }
        public int MaxModules { get; set; }
        public int ActiveUsers { get; set; }
        public int ActiveModules { get; set; }
        public bool HasDatabase { get; set; }
        public bool IsDatabaseHealthy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}

