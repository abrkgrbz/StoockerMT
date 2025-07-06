using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Features.Authentication.DTOs
{
    public class AuthenticationDtos
    {
        public record LoginRequest(
            string Email,
            string Password,
            string TenantCode
        );

        public record LoginResponse(
            string AccessToken,
            string RefreshToken,
            DateTime ExpiresAt,
            UserInfo User
        );

        public record UserInfo(
            int Id,
            string Email,
            string FullName,
            string TenantCode,
            string TenantName,
            List<string> Permissions
        );

        public record RefreshTokenRequest(
            string AccessToken,
            string RefreshToken
        );

        public record ChangePasswordRequest(
            string CurrentPassword,
            string NewPassword
        );

        // JWT Token Claims
        public static class CustomClaimTypes
        {
            public const string UserId = "uid";
            public const string TenantId = "tid";
            public const string TenantCode = "tcode";
            public const string Permissions = "permissions";
            public const string FullName = "fullName";
        }

        // Token Generation Result
        public class TokenGenerationResult
        {
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
            public DateTime ExpiresAt { get; set; }
        }
    }
}
