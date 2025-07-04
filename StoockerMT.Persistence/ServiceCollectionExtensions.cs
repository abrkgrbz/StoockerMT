using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StoockerMT.Application.Common.Interfaces;
using StoockerMT.Persistence.Contexts;
using StoockerMT.Persistence.Services;

namespace StoockerMT.Persistence
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            // Master Database
            services.AddDbContext<MasterDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("MasterConnection"),
                    b => b.MigrationsAssembly(typeof(MasterDbContext).Assembly.FullName)
                        .MigrationsHistoryTable("__MasterMigrationsHistory")));

            // Tenant Database Factory
            services.AddDbContextFactory<TenantDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("TenantConnection"),
                    b => b.MigrationsAssembly(typeof(TenantDbContext).Assembly.FullName)
                        .MigrationsHistoryTable("__TenantMigrationsHistory")));

            services.AddScoped<ITenantDatabaseService, TenantDatabaseService>();  

            return services;
        }
    }
}
