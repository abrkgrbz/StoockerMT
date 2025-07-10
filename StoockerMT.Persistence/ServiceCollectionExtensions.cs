using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Polly;
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
             
            services.AddSecurityServices(configuration);

            services.AddTenantServices();
            services.AddScoped<AuditableEntitySaveChangesInterceptor>();
            services.AddScoped<DomainEventDispatcherInterceptor>();
            services.AddScoped<SoftDeleteInterceptor>();
            services.AddScoped<PerformanceInterceptor>();
            services.AddScoped<ConnectionInterceptor>();

            services.Configure<SqlConnectionStringBuilder>(options =>
            {
                var resiliencyConfig = configuration.GetSection("ConnectionResiliency");

                options.ConnectTimeout = resiliencyConfig.GetValue<int>("CommandTimeoutSeconds", 30);
                options.ConnectRetryCount = resiliencyConfig.GetValue<int>("MaxRetryCount", 3);
                options.ConnectRetryInterval = 10;
                options.MinPoolSize = resiliencyConfig.GetValue<int>("MinPoolSize", 5);
                options.MaxPoolSize = resiliencyConfig.GetValue<int>("MaxPoolSize", 100);
                options.MultipleActiveResultSets = true;
            });
          
            services.AddDbContexts(configuration);
             
            services.AddRepositories();

            return services;
        }
        private static IServiceCollection AddTenantServices(this IServiceCollection services)
        {
            services.AddScoped<ITenantResolver, TenantResolver>();

            services.AddScoped<CurrentTenantService>();
            services.AddScoped<ICurrentTenantService>(sp => sp.GetRequiredService<CurrentTenantService>());
            services.AddScoped<ICurrentUserService>(sp => sp.GetRequiredService<CurrentTenantService>());

            return services;
        }
        private static IServiceCollection AddSecurityServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure Data Protection
            var dataProtectionConfig = configuration.GetSection("DataProtection");
            var keyStoragePath = dataProtectionConfig["KeyStoragePath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "Keys");

            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(keyStoragePath))
                .SetApplicationName("StoockerMT")
                .SetDefaultKeyLifetime(TimeSpan.FromDays(90));

            // Add Encryption Service
            services.AddSingleton<IEncryptionService, EncryptionService>();

            // Configure Azure Key Vault if available
            var keyVaultUri = configuration["KeyVault:Uri"];
            if (!string.IsNullOrEmpty(keyVaultUri))
            {
                services.Configure<KeyVaultOptions>(options =>
                {
                    options.Uri = keyVaultUri;
                    options.TenantId = configuration["KeyVault:TenantId"];
                    options.ClientId = configuration["KeyVault:ClientId"];
                    options.ClientSecret = configuration["KeyVault:ClientSecret"];
                });
            }

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
                        //sqlOptions.EnableRetryOnFailure(
                        //    maxRetryCount: 6,
                        //    maxRetryDelay: TimeSpan.FromSeconds(30),
                        //    errorNumbersToAdd: null);
                    })
                .AddInterceptors(
                    sp.GetRequiredService<AuditableEntitySaveChangesInterceptor>(),
                    sp.GetRequiredService<SoftDeleteInterceptor>(),
                    sp.GetRequiredService<PerformanceInterceptor>());
            });

            // Register TenantDbContext factory
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
                        //sqlOptions.EnableRetryOnFailure(
                        //    maxRetryCount: 6,
                        //    maxRetryDelay: TimeSpan.FromSeconds(30),
                        //    errorNumbersToAdd: null);
                    })
                .AddInterceptors(
                    sp.GetRequiredService<AuditableEntitySaveChangesInterceptor>(),
                    sp.GetRequiredService<SoftDeleteInterceptor>(),
                    sp.GetRequiredService<PerformanceInterceptor>());
            });

            // Register TenantDbContext
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

            // Services
            services.AddScoped<ITenantService, TenantService>();
            services.AddScoped<IEncryptionService, EncryptionService>();
            services.AddSingleton<IResilientDatabaseService, ResilientDatabaseService>();
            return services;
        }
    }
     
    public class KeyVaultOptions
    {
        public string Uri { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}