using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoockerMT.Application.Common.Interfaces;
using StoockerMT.Domain.Entities.MasterDb;
using StoockerMT.Domain.ValueObjects;
using StoockerMT.Persistence.Contexts;

namespace StoockerMT.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ITenantDatabaseService _tenantDbService;
        private readonly MasterDbContext _masterContext;
        private readonly IConfiguration _configuration;
        public TestController(ITenantDatabaseService tenantDbService, MasterDbContext masterContext, IConfiguration configuration)
        {
            _tenantDbService = tenantDbService;
            _masterContext = masterContext;
            _configuration = configuration;
        }

        [HttpPost("create-test-tenant")]
        public async Task<IActionResult> CreateTestTenant()
        {
            var tenantCode = new TenantCode("TEST001");
            var tenant = new Tenant("Test Company", tenantCode, "System");

            _masterContext.Tenants.Add(tenant);
            await _masterContext.SaveChangesAsync();

            var dbCreated = await _tenantDbService.CreateTenantDatabaseAsync(tenantCode.Value);

            if (dbCreated)
            {
                var connectionString = _configuration.GetConnectionString("MasterConnection")
                    .Replace("StoockerMT_Master", $"TenantDB_{tenantCode.Value}");

                tenant.UpdateConnectionString(connectionString);
                await _masterContext.SaveChangesAsync();

                return Ok("Test tenant created successfully!");
            }

            return BadRequest("Failed to create test tenant");
        }
    }
}
