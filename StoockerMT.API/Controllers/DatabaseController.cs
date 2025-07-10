using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using StoockerMT.Persistence.Contexts;
using System.Data;

namespace StoockerMT.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatabaseTestController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly MasterDbContext _masterDbContext;
        private readonly ILogger<DatabaseTestController> _logger;

        public DatabaseTestController(
            IConfiguration configuration,
            MasterDbContext masterDbContext,
            ILogger<DatabaseTestController> logger)
        {
            _configuration = configuration;
            _masterDbContext = masterDbContext;
            _logger = logger;
        }

        [HttpGet("test-connection")]
        public async Task<IActionResult> TestConnection()
        {
            var results = new Dictionary<string, object>();

            // Test Master Database Connection
            results["masterDatabase"] = await TestMasterDatabaseConnection();

            // Test Tenant Database Connection
            results["tenantDatabase"] = await TestTenantDatabaseConnection();
             

            // Overall status
            var allSuccess = results.Values.All(r => r is Dictionary<string, object> dict && dict["success"] is bool success && success);
            results["overallStatus"] = allSuccess ? "All connections successful" : "Some connections failed";

            return Ok(results);
        }

        [HttpGet("test-master")]
        public async Task<IActionResult> TestMasterDatabase()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("MasterConnection");

                if (string.IsNullOrEmpty(connectionString))
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "Master connection string is not configured"
                    });
                }

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                // Test query
                using var command = new SqlCommand("SELECT @@VERSION AS Version, DB_NAME() AS DatabaseName, GETDATE() AS ServerTime", connection);
                using var reader = await command.ExecuteReaderAsync();

                var info = new Dictionary<string, object>();
                if (await reader.ReadAsync())
                {
                    info["version"] = reader["Version"].ToString();
                    info["databaseName"] = reader["DatabaseName"].ToString();
                    info["serverTime"] = reader["ServerTime"].ToString();
                }

                // Get table count
                await reader.CloseAsync();
                command.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
                var tableCount = await command.ExecuteScalarAsync();
                info["tableCount"] = tableCount;

                return Ok(new
                {
                    success = true,
                    connectionState = connection.State.ToString(),
                    serverInfo = info,
                    connectionString = MaskConnectionString(connectionString)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to master database");
                return Ok(new
                {
                    success = false,
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpGet("test-tenant")]
        public async Task<IActionResult> TestTenantDatabase()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("TenantConnection");

                if (string.IsNullOrEmpty(connectionString))
                {
                    return BadRequest(new
                    {
                        success = false,
                        error = "Tenant connection string is not configured"
                    });
                }

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                // Get database info
                var info = new Dictionary<string, object>();
                using var command = new SqlCommand(@"
                    SELECT 
                        DB_NAME() AS DatabaseName,
                        SERVERPROPERTY('ServerName') AS ServerName,
                        SERVERPROPERTY('MachineName') AS MachineName,
                        SERVERPROPERTY('IsClustered') AS IsClustered,
                        SERVERPROPERTY('ProductVersion') AS ProductVersion
                ", connection);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    info["databaseName"] = reader["DatabaseName"].ToString();
                    info["serverName"] = reader["ServerName"].ToString();
                    info["machineName"] = reader["MachineName"].ToString();
                    info["isClustered"] = reader["IsClustered"].ToString();
                    info["productVersion"] = reader["ProductVersion"].ToString();
                }

                return Ok(new
                {
                    success = true,
                    connectionState = connection.State.ToString(),
                    serverInfo = info,
                    connectionString = MaskConnectionString(connectionString)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to tenant database");
                return Ok(new
                {
                    success = false,
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        
        [HttpGet("test-all")]
        public async Task<IActionResult> TestAllConnections()
        {
            var results = new List<object>();

            // Test raw SQL connections
            foreach (var connName in new[] { "MasterConnection", "TenantConnection" })
            {
                var connString = _configuration.GetConnectionString(connName);
                if (string.IsNullOrEmpty(connString))
                {
                    results.Add(new
                    {
                        connectionName = connName,
                        success = false,
                        error = "Connection string not configured"
                    });
                    continue;
                }

                try
                {
                    using var connection = new SqlConnection(connString);
                    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                    await connection.OpenAsync();
                    stopwatch.Stop();

                    results.Add(new
                    {
                        connectionName = connName,
                        success = true,
                        connectionTime = $"{stopwatch.ElapsedMilliseconds}ms",
                        state = connection.State.ToString(),
                        database = connection.Database,
                        dataSource = connection.DataSource
                    });
                }
                catch (Exception ex)
                {
                    results.Add(new
                    {
                        connectionName = connName,
                        success = false,
                        error = ex.Message
                    });
                }
            }

            return Ok(new
            {
                timestamp = DateTime.UtcNow,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                connections = results
            });
        }

        private async Task<Dictionary<string, object>> TestMasterDatabaseConnection()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("MasterConnection");
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                return new Dictionary<string, object>
                {
                    ["success"] = true,
                    ["database"] = connection.Database,
                    ["state"] = connection.State.ToString()
                };
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object>
                {
                    ["success"] = false,
                    ["error"] = ex.Message
                };
            }
        }

        private async Task<Dictionary<string, object>> TestTenantDatabaseConnection()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("TenantConnection");
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                return new Dictionary<string, object>
                {
                    ["success"] = true,
                    ["database"] = connection.Database,
                    ["state"] = connection.State.ToString()
                };
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object>
                {
                    ["success"] = false,
                    ["error"] = ex.Message
                };
            }
        }
         

        private string MaskConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                return connectionString;

            var builder = new SqlConnectionStringBuilder(connectionString);
            if (!string.IsNullOrEmpty(builder.Password))
            {
                builder.Password = "***MASKED***";
            }
            return builder.ToString();
        }
    }
}