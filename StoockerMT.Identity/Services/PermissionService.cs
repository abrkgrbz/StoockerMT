using StoockerMT.Application.Common.Security;
using StoockerMT.Domain.Repositories.MasterDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace StoockerMT.Identity.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly ITenantUserRepository _userRepository;
        private readonly IModulePermissionRepository _permissionRepository;

        public PermissionService(
            ITenantUserRepository userRepository,
            IModulePermissionRepository permissionRepository)
        {
            _userRepository = userRepository;
            _permissionRepository = permissionRepository;
        }

        public async Task<List<string>> GetUserPermissionsAsync(int userId, int tenantId)
        {
            var userPermissions = await _userRepository
                .GetQueryable()
                .Where(u => u.Id == userId && u.TenantId == tenantId)
                .SelectMany(u => u.Permissions)
                .Select(p => p.Permission.Code)
                .ToListAsync();

            return userPermissions;
        }

        public async Task<bool> HasPermissionAsync(int userId, int tenantId, string permission)
        {
            return await _userRepository
                .GetQueryable()
                .Where(u => u.Id == userId && u.TenantId == tenantId)
                .SelectMany(u => u.Permissions)
                .AnyAsync(p => p.Permission.Code == permission);
        }

        public async Task<bool> HasModuleAccessAsync(int userId, int tenantId, string moduleCode)
        {
            return await _userRepository
                .GetQueryable()
                .Where(u => u.Id == userId && u.TenantId == tenantId)
                .SelectMany(u => u.Permissions)
                .AnyAsync(p => p.Permission.Module.Code == moduleCode);
        }
    }
}
