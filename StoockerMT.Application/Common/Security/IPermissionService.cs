using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Security
{
    public interface IPermissionService
    {
        Task<List<string>> GetUserPermissionsAsync(int userId, int tenantId);
        Task<bool> HasPermissionAsync(int userId, int tenantId, string permission);
        Task<bool> HasModuleAccessAsync(int userId, int tenantId, string moduleCode);
    }
}
