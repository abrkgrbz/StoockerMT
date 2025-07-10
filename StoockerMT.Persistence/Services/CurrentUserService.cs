using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using StoockerMT.Application.Common.Interfaces;

namespace StoockerMT.Persistence.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId
        {
            get
            {
                // Try different claim types
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    userId = _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(userId))
                    userId = _httpContextAccessor.HttpContext?.User?.FindFirst("id")?.Value;

                return userId;
            }
        }

        public string? UserName
        {
            get
            {
                var userName = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(userName))
                    userName = _httpContextAccessor.HttpContext?.User?.FindFirst("name")?.Value;

                if (string.IsNullOrEmpty(userName))
                    userName = _httpContextAccessor.HttpContext?.User?.FindFirst("username")?.Value;

                return userName;
            }
        }

        public string? Email
        {
            get
            {
                var email = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(email))
                    email = _httpContextAccessor.HttpContext?.User?.FindFirst("email")?.Value;

                return email;
            }
        }

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}