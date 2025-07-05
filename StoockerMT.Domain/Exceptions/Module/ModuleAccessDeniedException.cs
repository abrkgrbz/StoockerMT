using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Exceptions.Module
{
    public class ModuleAccessDeniedException : DomainException
    {
        public int UserId { get; }
        public string ModuleCode { get; }
        public string RequiredPermission { get; }

        public ModuleAccessDeniedException(int userId, string moduleCode, string requiredPermission)
            : base("MODULE_ACCESS_DENIED",
                $"User '{userId}' does not have permission '{requiredPermission}' for module '{moduleCode}'.")
        {
            UserId = userId;
            ModuleCode = moduleCode;
            RequiredPermission = requiredPermission;
        }
    }
}
