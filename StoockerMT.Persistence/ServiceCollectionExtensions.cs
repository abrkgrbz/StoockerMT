using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StoockerMT.Application.Common.Interfaces;
using StoockerMT.Application.Common.Interfaces.Services;
using StoockerMT.Domain.Repositories.MasterDb;
using StoockerMT.Domain.Repositories.TenantDb;
using StoockerMT.Domain.Repositories.UnitOfWork;
using StoockerMT.Persistence.Contexts;
using StoockerMT.Persistence.Interceptors;
using StoockerMT.Persistence.Repositories.MasterDb;
using StoockerMT.Persistence.Repositories.TenantDb;
using StoockerMT.Persistence.Repositories.UnitOfWork;
using StoockerMT.Persistence.Services;

namespace StoockerMT.Persistence
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        { 
            services.AddHttpContextAccessor();
             
            services.AddScoped<AuditableEntitySaveChangesInterceptor>();
            services.AddScoped<DomainEventDispatcherInterceptor>();
            services.AddScoped<SoftDeleteInterceptor>();
            services.AddScoped<PerformanceInterceptor>();
             
            services.AddDbContexts(configuration);
 
            services.AddRepositories();   

            return services;
        }

        private static IServiceCollection AddDbContexts(this IServiceCollection services, IConfiguration configuration)
        { 
            services.AddDbContext<MasterDbContext>((sp, options) =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("MasterConnection"),
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(MasterDbContext).Assembly.FullName);
                        sqlOptions.MigrationsHistoryTable("__MasterMigrationsHistory");
                        sqlOptions.CommandTimeout(30);
                    })
                    .AddInterceptors(
                        sp.GetRequiredService<AuditableEntitySaveChangesInterceptor>(),
                        sp.GetRequiredService<SoftDeleteInterceptor>(),
                        sp.GetRequiredService<PerformanceInterceptor>());
            });
             
            services.AddDbContextFactory<TenantDbContext>((sp, options) =>
            {
                var currentTenantService = sp.GetService<ICurrentTenantService>();
                var connectionString = currentTenantService?.ConnectionString ??
                                     configuration.GetConnectionString("TenantConnection");

                options.UseSqlServer(
                    connectionString,
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(TenantDbContext).Assembly.FullName);
                        sqlOptions.MigrationsHistoryTable("__TenantMigrationsHistory");
                        sqlOptions.CommandTimeout(30);
                    })
                    .AddInterceptors(
                        sp.GetRequiredService<AuditableEntitySaveChangesInterceptor>(),
                        sp.GetRequiredService<SoftDeleteInterceptor>(),
                        sp.GetRequiredService<PerformanceInterceptor>());
            });
             
            services.AddScoped(sp =>
            {
                var factory = sp.GetRequiredService<IDbContextFactory<TenantDbContext>>();
                return factory.CreateDbContext();
            });

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // Master DB Repositories
            services.AddScoped<ITenantRepository, TenantRepository>();
            services.AddScoped<IModuleRepository, ModuleRepository>();
            services.AddScoped<ITenantUserRepository, TenantUserRepository>();
            services.AddScoped<ITenantModuleSubscriptionRepository, TenantModuleSubscriptionRepository>();
            services.AddScoped<ITenantInvoiceRepository, TenantInvoiceRepository>();
            services.AddScoped<IModulePermissionRepository, ModulePermissionRepository>();
            services.AddScoped<ITenantModuleUsageRepository, TenantModuleUsageRepository>();

            // Tenant DB Repositories
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IJournalEntryRepository, JournalEntryRepository>();
            services.AddScoped<IInventoryMovementRepository, InventoryMovementRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IPositionRepository, PositionRepository>();
            services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();

            // Unit of Work
            services.AddScoped<IMasterDbUnitOfWork, MasterDbUnitOfWork>();
            services.AddScoped<ITenantDbUnitOfWork, TenantDbUnitOfWork>();

            services.AddScoped<ITenantService, TenantService>();

            return services;
        }
          
    }
}
