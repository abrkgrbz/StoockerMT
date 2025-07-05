using StoockerMT.Domain.Repositories.MasterDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Repositories.UnitOfWork
{
    public interface IMasterDbUnitOfWork : IUnitOfWork
    {
        ITenantRepository Tenants { get; }
        IModuleRepository Modules { get; }
        ITenantUserRepository TenantUsers { get; }
        ITenantModuleSubscriptionRepository Subscriptions { get; }
        ITenantInvoiceRepository Invoices { get; }
        IModulePermissionRepository Permissions { get; }
        ITenantModuleUsageRepository ModuleUsages { get; }
    }
}
