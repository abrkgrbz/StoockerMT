using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StoockerMT.Application.Features.Authentication.DTOs.AuthenticationDtos;

namespace StoockerMT.Application.Common.Security
{
    public interface IAuthenticationService
    {
        Task<LoginResponse> LoginAsync(string email, string password, string tenantCode);
        Task<LoginResponse> RefreshTokenAsync(string accessToken, string refreshToken);
        Task<bool> LogoutAsync(int userId);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    }
}
