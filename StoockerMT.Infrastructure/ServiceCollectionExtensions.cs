using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StoockerMT.Application.Common.Interfaces.Services;
using StoockerMT.Application.Common.Interfaces;
using StoockerMT.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Persistence.Services;

namespace StoockerMT.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
           
            services.AddScoped<IFileStorageService, LocalFileStorageService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IDateTime, DateTimeService>();
            services.AddCaching(configuration);
            return services;
        }

        private static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
        {
            var cacheSettings = configuration.GetSection("Cache");
            var useRedis = cacheSettings.GetValue<bool>("UseRedis");

            if (useRedis)
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = cacheSettings["RedisConnection"];
                    options.InstanceName = cacheSettings["InstanceName"] ?? "StoockerMT";
                });
            }
            else
            {
                services.AddDistributedMemoryCache();
            }

            services.AddMemoryCache();

            return services;
        }
    }
}
