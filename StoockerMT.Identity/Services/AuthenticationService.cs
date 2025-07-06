using StoockerMT.Application.Common.Exceptions;
using StoockerMT.Application.Common.Security;
using StoockerMT.Domain.Repositories.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoockerMT.Domain.Entities.MasterDb;
using StoockerMT.Domain.ValueObjects;
using static StoockerMT.Application.Features.Authentication.DTOs.AuthenticationDtos;

namespace StoockerMT.Identity.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IMasterDbUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IPasswordService _passwordService;
        private readonly IPermissionService _permissionService;

        public AuthenticationService(
            IMasterDbUnitOfWork unitOfWork,
            ITokenService tokenService,
            IPasswordService passwordService,
            IPermissionService permissionService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _passwordService = passwordService;
            _permissionService = permissionService;
        }

        public async Task<LoginResponse> LoginAsync(string email, string password, string tenantCode)
        {
            // Find tenant
            var tenant = await _unitOfWork.Tenants
                .GetQueryable()
                .FirstOrDefaultAsync(t => t.Code == new TenantCode(tenantCode));

            if (tenant == null)
                throw new ValidationException();

            // Check tenant status
            if (tenant.Status != Domain.Enums.TenantStatus.Active)
                throw new ValidationException();

            // Find user
            var user = await _unitOfWork.TenantUsers
                .GetQueryable()
                .Include(u => u.Permissions)
                .ThenInclude(p => p.Permission)
                .FirstOrDefaultAsync(u => u.Email == email && u.TenantId == tenant.Id);

            if (user == null)
                throw new ValidationException();
 
             
            if (!_passwordService.VerifyPassword(password, user.PasswordHash))
            { 
                user.FailedLoginAttempts++;

                if (user.FailedLoginAttempts >= 5)
                {
                    user.UpdateLocked(user.FullName,true);
                    user.UpdateLockedUntil(user.FullName, DateTime.UtcNow.AddMinutes(15));
                }

                await _unitOfWork.SaveChangesAsync();
                throw new ValidationException();
            }
             
            user.FailedLoginAttempts = 0;
            user.LastLoginDate = DateTime.UtcNow;
             
            var permissions = await _permissionService.GetUserPermissionsAsync(user.Id, tenant.Id);
             
            var tokenResult = _tokenService.GenerateToken(user, tenant, permissions);
             
            user.RefreshToken = tokenResult.RefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _unitOfWork.SaveChangesAsync();

            return new LoginResponse(
                tokenResult.AccessToken,
                tokenResult.RefreshToken,
                tokenResult.ExpiresAt,
                new UserInfo(
                    user.Id,
                    user.Email,
                    user.FullName,
                    tenant.Code.Value,
                    tenant.Name,
                    permissions
                )
            );
        }

        public async Task<LoginResponse> RefreshTokenAsync(string accessToken, string refreshToken)
        {
            var principal = _tokenService.ValidateToken(accessToken);
            if (principal == null)
                throw new ValidationException();

            var userId = Convert.ToInt64(principal.FindFirst(CustomClaimTypes.UserId)?.Value ?? "");
            var tenantId = Convert.ToInt64(principal.FindFirst(CustomClaimTypes.TenantId)?.Value ?? "");

            var user = await _unitOfWork.TenantUsers
                .GetQueryable()
                .Include(u => u.Tenant)
                .FirstOrDefaultAsync(u => u.Id == userId && u.TenantId == tenantId);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new ValidationException();

            var permissions = await _permissionService.GetUserPermissionsAsync(user.Id, user.TenantId);
            var tokenResult = _tokenService.GenerateToken(user, user.Tenant, permissions);

            user.RefreshToken = tokenResult.RefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _unitOfWork.SaveChangesAsync();

            return new LoginResponse(
                tokenResult.AccessToken,
                tokenResult.RefreshToken,
                tokenResult.ExpiresAt,
                new UserInfo(
                    user.Id,
                    user.Email,
                    user.FullName,
                    user.Tenant.Code.Value,
                    user.Tenant.Name,
                    permissions
                )
            );
        }

        public async Task<bool> LogoutAsync(int userId)
        {
            var user = await _unitOfWork.TenantUsers.GetByIdAsync(userId);
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                await _unitOfWork.SaveChangesAsync();
            }
            return true;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _unitOfWork.TenantUsers.GetByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User not found");

            if (!_passwordService.VerifyPassword(currentPassword, user.PasswordHash))
                throw new ValidationException();

            if (!_passwordService.ValidatePasswordStrength(newPassword))
                throw new ValidationException();

            user.UpdatePassword(_passwordService.HashPassword(newPassword),user.FullName);
            user.PasswordChangeDate = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
