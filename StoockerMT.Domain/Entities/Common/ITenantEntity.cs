using StoockerMT.Domain.Entities.MasterDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Domain.ValueObjects;

namespace StoockerMT.Domain.Entities.Common
{
    public interface ITenantEntity
    {
        TenantId TenantId { get; set; }
    }
}
