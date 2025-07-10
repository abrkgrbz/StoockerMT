using StoockerMT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Specifications.MasterDb.Tenant
{
    public class ActiveTenantsSpecification : BaseSpecification<Domain.Entities.MasterDb.Tenant>
    {
        public ActiveTenantsSpecification()
            : base(t => t.Status == TenantStatus.Active && !t.IsDeleted)
        {
            ApplyOrderBy(t => t.Name);
        }
    }
}
