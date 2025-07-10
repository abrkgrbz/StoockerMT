using StoockerMT.Domain.Entities.MasterDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Interfaces.Services
{
    public interface ITenantResolver
    { 
        Task<Tenant> ResolveByIdAsync(int tenantId);
        Task<Tenant> ResolveByCodeAsync(string tenantCode);
        Task<Tenant> ResolveByDomainAsync(string domain);
        Task<Tenant> ResolveByHeaderAsync(string headerValue);
        Task<Tenant> ResolveByUserAsync(string userEmail);
         
        Task<Tenant> ResolveAsync(string identifier);
    }
}
