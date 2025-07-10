using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using StoockerMT.Application.Common.Interfaces.Services;

namespace StoockerMT.Persistence.Services
{ }

public class ResilientDatabaseService : IResilientDatabaseService
{
    private readonly IAsyncPolicy _retryPolicy;
    private readonly ILogger<ResilientDatabaseService> _logger;

    public ResilientDatabaseService(ILogger<ResilientDatabaseService> logger)
    {
        _logger = logger;
        _retryPolicy = StoockerMT.Persistence.Policies.SqlRetryPolicy.CreateAsyncRetryPolicyWithCircuitBreaker(logger);
    }

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _retryPolicy.ExecuteAsync(async (ct) => await operation(), cancellationToken);
        }
        catch (BrokenCircuitException ex)
        {
            _logger.LogError(ex, "Circuit breaker is open. Database operations are currently unavailable");
            throw new InvalidOperationException("Database is currently unavailable. Please try again later.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database operation failed after all retry attempts");
            throw;
        }
    }

    public async Task ExecuteAsync(Func<Task> operation, CancellationToken cancellationToken = default)
    {
        try
        {
            await _retryPolicy.ExecuteAsync(async (ct) => await operation(), cancellationToken);
        }
        catch (BrokenCircuitException ex)
        {
            _logger.LogError(ex, "Circuit breaker is open. Database operations are currently unavailable");
            throw new InvalidOperationException("Database is currently unavailable. Please try again later.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database operation failed after all retry attempts");
            throw;
        }
    }

    public async Task<T> ExecuteInTransactionAsync<T>(
        Func<Task<T>> operation,
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(async () =>
        {
            // Transaction will be handled by the caller's DbContext
            return await operation();
        }, cancellationToken);
    }

    public async Task<bool> TestConnectionAsync(string connectionString, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _retryPolicy.ExecuteAsync(async (ct) =>
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync(ct);

                // Test with a simple query
                using var command = new SqlCommand("SELECT 1", connection);
                var result = await command.ExecuteScalarAsync(ct);

                return result != null && (int)result == 1;
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Connection test failed for connection string");
            return false;
        }
    }

    public async Task<bool> IsDatabaseHealthyAsync(DbContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _retryPolicy.ExecuteAsync(async (ct) =>
            {
                // Test database connection
                var canConnect = await context.Database.CanConnectAsync(ct);
                if (!canConnect)
                    return false;

                // Test a simple query
                var result = await context.Database.ExecuteSqlRawAsync("SELECT 1", ct);

                // Check for pending migrations
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync(ct);
                if (pendingMigrations.Any())
                {
                    _logger.LogWarning("Database has {Count} pending migrations", pendingMigrations.Count());
                    return false;
                }

                return true;
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check failed");
            return false;
        }
    }
} 