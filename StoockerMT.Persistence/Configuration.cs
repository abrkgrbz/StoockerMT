using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace StoockerMT.Persistence
{
    public static class Configuration
    {
        private static IConfigurationRoot _configuration;
        private static readonly object _lock = new object();

        private static IConfigurationRoot GetConfiguration()
        {
            if (_configuration == null)
            {
                lock (_lock)
                {
                    if (_configuration == null)
                    {
                        var builder = new ConfigurationBuilder(); 
                        var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
                        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

                        if (isDocker)
                        {  
                            builder.SetBasePath(Directory.GetCurrentDirectory());
                        }
                        else
                        { 
                            var currentDir = Directory.GetCurrentDirectory();
                            var apiProjectPath = "";
                             
                            if (currentDir.Contains("StoockerMT.Persistence"))
                            {
                                apiProjectPath = Path.Combine(currentDir, "../StoockerMT.API");
                            } 
                            else if (File.Exists(Path.Combine(currentDir, "StoockerMT.sln")))
                            {
                                apiProjectPath = Path.Combine(currentDir, "StoockerMT.API");
                            } 
                            else
                            {
                                apiProjectPath = currentDir;
                            }

                            if (Directory.Exists(apiProjectPath))
                            {
                                builder.SetBasePath(apiProjectPath);
                            }
                            else
                            {
                                builder.SetBasePath(currentDir);
                            }
                        }

                        // Configuration dosyalarını ekle
                        builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                        builder.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);

                        // Environment variables'ları ekle (Docker için önemli)
                        builder.AddEnvironmentVariables();

                        _configuration = builder.Build();
                    }
                }
            }
            return _configuration;
        }

        public static string ConnectionStringMasterDb
        {
            get
            {
                try
                {
                    var configuration = GetConfiguration();
                     
                    var envConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__MasterConnection");
                    if (!string.IsNullOrEmpty(envConnectionString))
                    {
                        return envConnectionString;
                    } 
                    var connectionString = configuration.GetConnectionString("MasterConnection");

                    if (string.IsNullOrEmpty(connectionString))
                    { 
                        var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
                        var server = isDocker ? "sqlserver" : "localhost";
                        connectionString = $"Server={server},1433;Database=StoockerMT_Master;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=True;MultipleActiveResultSets=true";
                    }

                    return connectionString;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error getting master connection string: {e.Message}");
                    // Fallback connection string
                    return "Server=localhost,1433;Database=StoockerMT_Master;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=True;MultipleActiveResultSets=true";
                }
            }
        }

        public static string ConnectionStringTenantDb
        {
            get
            {
                try
                {
                    var configuration = GetConfiguration();

                    // Önce environment variable'dan kontrol et
                    var envConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__TenantConnection");
                    if (!string.IsNullOrEmpty(envConnectionString))
                    {
                        return envConnectionString;
                    }

                    // Sonra configuration'dan al
                    var connectionString = configuration.GetConnectionString("TenantConnection");

                    if (string.IsNullOrEmpty(connectionString))
                    {
                        // Default connection string template
                        var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
                        var server = isDocker ? "sqlserver" : "localhost";
                        connectionString = $"Server={server},1433;Database=StoockerMT_Tenant_{{0}};User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=True;MultipleActiveResultSets=true";
                    }

                    return connectionString;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error getting tenant connection string: {e.Message}");
                    // Fallback connection string template
                    return "Server=localhost,1433;Database=StoockerMT_Tenant_{0};User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=True;MultipleActiveResultSets=true";
                }
            }
        }
         
        public static string GetValue(string key, string defaultValue = null)
        {
            try
            {
                var configuration = GetConfiguration();
                return configuration[key] ?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }
         
        public static IConfigurationSection GetSection(string key)
        {
            var configuration = GetConfiguration();
            return configuration.GetSection(key);
        }
         
        public static bool IsRunningInDocker()
        {
            return Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true" ||
                   File.Exists("/.dockerenv");
        }
    }
}