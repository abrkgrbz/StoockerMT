using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Security
{
    public interface ITokenService
    {
        Task<string> GenerateJwtToken(int userId, int tenantId, string email, IEnumerable<string> roles);
        Task<ClaimsPrincipal> ValidateToken(string token);
        Task<string> RefreshToken(string token, string refreshToken);
    }
}
