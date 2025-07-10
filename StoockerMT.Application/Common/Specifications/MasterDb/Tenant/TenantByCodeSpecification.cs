using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Specifications.MasterDb.Tenant
{
    public class TenantByCodeSpecification : BaseSpecification<Domain.Entities.MasterDb.Tenant>
    {
        public TenantByCodeSpecification(string code)
            : base(t => t.Code.Value == code && !t.IsDeleted)
        {
            AddInclude(t => t.ModuleSubscriptions);
            AddInclude("ModuleSubscriptions.Module");
            ApplyNoTracking();
        }
    }
}
