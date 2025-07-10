using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoockerMT.Application.Common.Interfaces;

namespace StoockerMT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantAwareController : ControllerBase
    {
        protected readonly ICurrentTenantService _currentTenantService;

        protected TenantAwareController(ICurrentTenantService currentTenantService)
        {
            _currentTenantService = currentTenantService;
        }

        protected int CurrentTenantId => _currentTenantService.TenantId ?? 0;
        protected string CurrentTenantCode => _currentTenantService.TenantCode;
        protected int? CurrentUserId => _currentTenantService.UserId;
        protected bool IsTenantAdmin => _currentTenantService.IsTenantAdmin;

        protected async Task<IActionResult> ValidateTenantAccessAsync()
        {
            if (!_currentTenantService.HasTenant())
            {
                return BadRequest("Tenant identification required");
            }

            if (_currentTenantService.IsAuthenticated() && !await _currentTenantService.ValidateTenantAccessAsync())
            {
                return Forbid("Access denied to this tenant");
            }

            return null;
        }
    }
}
