using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace StoockerMT.Persistence.Policies
{
    public static class SqlRetryPolicy
    {
        private static readonly int[] SqlTransientErrorNumbers = new[]
        {
            49918, // Cannot process request. Not enough resources to process request
            49919, // Cannot process create or update request. Too many create or update operations in progress
            49920, // Cannot process request. Too many operations in progress
            4060,  // Cannot open database requested by the login
            40143, // The service has encountered an error processing your request
            40197, // The service has encountered an error processing your request
            40501, // The service is currently busy
            40540, // The service has encountered an error processing your request
            40613, // Database is not currently available
            64,    // A connection was successfully established but then an error occurred during the login process
            233,   // The client was unable to establish a connection
            20,    // The instance of SQL Server does not support encryption
            121,   // The semaphore timeout period has expired
            1205,  // Deadlock victim
            -2,    // Timeout expired
            2,     // Network error
            5,     // Server is too busy
            8,     // Server closed the connection
            14,    // Connection was terminated
            18,    // Connection is busy
            19,    // Physical connection is not usable
            64     // Login failed
        };
         
        public static AsyncRetryPolicy CreateAsyncRetryPolicy(ILogger logger = null)
        {
            return Policy
                .Handle<SqlException>(IsTransientException)
                .Or<TimeoutException>()
                .Or<InvalidOperationException>(ex => ex.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase))
                .WaitAndRetryAsync(
                    retryCount: 6,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Exponential backoff
                    onRetry: (outcome, timespan, retryCount, context) =>
                    {
                        var exception = outcome.InnerException;
                        logger?.LogWarning(
                            exception,
                            "SQL operation retry attempt {RetryCount} after {TimeSpan}ms delay. Error: {ErrorMessage}",
                            retryCount,
                            timespan.TotalMilliseconds,
                            exception.Message);
                    });
        }
         
        public static IAsyncPolicy CreateAsyncRetryPolicyWithCircuitBreaker(ILogger logger = null)
        {
            var retryPolicy = CreateAsyncRetryPolicy(logger);

            var circuitBreakerPolicy = Policy
                .Handle<SqlException>(IsTransientException)
                .CircuitBreakerAsync( 
                   5,
                   TimeSpan.FromSeconds(30),
                    onBreak: (outcome, duration) =>
                    {
                        logger?.LogError(
                            outcome.InnerException,
                            "Circuit breaker opened for {Duration}s due to consecutive failures",
                            duration.TotalSeconds);
                    },
                    onReset: () =>
                    {
                        logger?.LogInformation("Circuit breaker reset, normal operation resumed");
                    });

            return Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);
        }

         
        public static IExecutionStrategy CreateExecutionStrategy(ExecutionStrategyDependencies dependencies)
        {
            return new CustomSqlRetryExecutionStrategy(dependencies);
        }

        // Check if SQL exception is transient
        private static bool IsTransientException(SqlException sqlException)
        {
            if (sqlException.InnerException is SqlException innerException)
            {
                return IsTransientException(innerException);
            }

            foreach (SqlError error in sqlException.Errors)
            {
                if (Array.Exists(SqlTransientErrorNumbers, num => num == error.Number))
                {
                    return true;
                }
            }

            return false;
        }


        public class CustomSqlRetryExecutionStrategy : ExecutionStrategy
        {
            private readonly int _maxRetryCount;
            private readonly TimeSpan _maxRetryDelay;

            public CustomSqlRetryExecutionStrategy(ExecutionStrategyDependencies dependencies)
                : this(dependencies, maxRetryCount: 6, maxRetryDelay: TimeSpan.FromSeconds(30))
            {
            }

            public CustomSqlRetryExecutionStrategy(
                ExecutionStrategyDependencies dependencies,
                int maxRetryCount,
                TimeSpan maxRetryDelay)
                : base(dependencies, maxRetryCount, maxRetryDelay)
            {
                _maxRetryCount = maxRetryCount;
                _maxRetryDelay = maxRetryDelay;
            }

            protected override bool ShouldRetryOn(Exception exception)
            {
                if (exception is SqlException sqlException)
                {
                    return SqlRetryPolicy.IsTransientException(sqlException);
                }

                if (exception is TimeoutException)
                {
                    return true;
                }

                if (exception?.InnerException != null)
                {
                    return ShouldRetryOn(exception.InnerException);
                }

                return false;
            }

            protected override TimeSpan? GetNextDelay(Exception lastException)
            {
                var baseDelay = base.GetNextDelay(lastException);

                // Custom delay logic based on exception type
                if (lastException is SqlException sqlEx)
                {
                    // Deadlock: shorter retry delay
                    if (sqlEx.Number == 1205)
                    {
                        return TimeSpan.FromMilliseconds(100);
                    }

                    // Timeout: longer retry delay
                    if (sqlEx.Number == -2)
                    {
                        return TimeSpan.FromSeconds(5);
                    }
                }

                return baseDelay;
            }
        }
    } 
}