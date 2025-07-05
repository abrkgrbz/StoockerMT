using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Interfaces
{
    public interface ICurrentTenantService
    {
        int? TenantId { get; }
        string? TenantCode { get; }
        string? ConnectionString { get; }
        bool HasTenant { get; }
    }

}
