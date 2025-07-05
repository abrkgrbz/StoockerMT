using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Exceptions.Module
{
    public class ModuleNotSubscribedException : DomainException
    {
        public int TenantId { get; }
        public int ModuleId { get; }

        public ModuleNotSubscribedException(int tenantId, int moduleId)
            : base("MODULE_NOT_SUBSCRIBED",
                $"Tenant '{tenantId}' is not subscribed to module '{moduleId}'.")
        {
            TenantId = tenantId;
            ModuleId = moduleId;
        }
    }

}
