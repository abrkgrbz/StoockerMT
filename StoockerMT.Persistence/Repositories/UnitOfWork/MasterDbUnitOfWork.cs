using StoockerMT.Domain.Repositories.MasterDb;
using StoockerMT.Domain.Repositories.UnitOfWork;
using StoockerMT.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Persistence.Repositories.MasterDb;

namespace StoockerMT.Persistence.Repositories.UnitOfWork
{
    public class MasterDbUnitOfWork : Common.UnitOfWork, IMasterDbUnitOfWork
    {
        private readonly MasterDbContext _context;

        // Lazy initialization of repositories
        private ITenantRepository? _tenants;
        private IModuleRepository? _modules;
        private ITenantUserRepository? _tenantUsers;
        private ITenantModuleSubscriptionRepository? _subscriptions;
        private ITenantInvoiceRepository? _invoices;
        private IModulePermissionRepository? _permissions;
        private ITenantModuleUsageRepository? _moduleUsages;

        public MasterDbUnitOfWork(MasterDbContext context) : base(context)
        {
            _context = context;
        }

        public ITenantRepository Tenants =>
            _tenants ??= new TenantRepository(_context);

        public IModuleRepository Modules =>
            _modules ??= new ModuleRepository(_context);

        public ITenantUserRepository TenantUsers =>
        _tenantUsers ??= new TenantUserRepository(_context);

        public ITenantModuleSubscriptionRepository Subscriptions =>
            _subscriptions ??= new TenantModuleSubscriptionRepository(_context);

        public ITenantInvoiceRepository Invoices =>
        _invoices ??= new TenantInvoiceRepository(_context);

        public IModulePermissionRepository Permissions =>
        _permissions ??= new ModulePermissionRepository(_context);

        public ITenantModuleUsageRepository ModuleUsages =>
            _moduleUsages ??= new TenantModuleUsageRepository(_context);
    }
}
