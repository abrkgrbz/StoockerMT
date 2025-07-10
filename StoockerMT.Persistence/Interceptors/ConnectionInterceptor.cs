using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace StoockerMT.Persistence.Interceptors
{
    public class ConnectionInterceptor : DbConnectionInterceptor
    {
        private readonly ILogger<ConnectionInterceptor> _logger;
        private int _activeConnections = 0;
        private int _failedConnections = 0;
        private readonly object _lock = new object();

        public ConnectionInterceptor(ILogger<ConnectionInterceptor> logger)
        {
            _logger = logger;
        }

        public int ActiveConnections => _activeConnections;
        public int FailedConnections => _failedConnections;

        public override InterceptionResult ConnectionOpening(
            DbConnection connection,
            ConnectionEventData eventData,
            InterceptionResult result)
        {
            _logger.LogDebug("Opening connection to database: {Database}", connection.Database);
            return base.ConnectionOpening(connection, eventData, result);
        }

        public override async ValueTask<InterceptionResult> ConnectionOpeningAsync(
            DbConnection connection,
            ConnectionEventData eventData,
            InterceptionResult result,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Opening connection to database: {Database}", connection.Database);
            return await base.ConnectionOpeningAsync(connection, eventData, result, cancellationToken);
        }

        public override void ConnectionOpened(
            DbConnection connection,
            ConnectionEndEventData eventData)
        {
            lock (_lock)
            {
                _activeConnections++;
            }

            _logger.LogDebug(
                "Connection opened successfully. Active connections: {ActiveConnections}. Duration: {Duration}ms",
                _activeConnections,
                eventData.Duration.TotalMilliseconds);

            base.ConnectionOpened(connection, eventData);
        }

        public override async Task ConnectionOpenedAsync(
            DbConnection connection,
            ConnectionEndEventData eventData,
            CancellationToken cancellationToken = default)
        {
            lock (_lock)
            {
                _activeConnections++;
            }

            _logger.LogDebug(
                "Connection opened successfully. Active connections: {ActiveConnections}. Duration: {Duration}ms",
                _activeConnections,
                eventData.Duration.TotalMilliseconds);

            await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
        }

        public override void ConnectionClosed(
            DbConnection connection,
            ConnectionEndEventData eventData)
        {
            lock (_lock)
            {
                if (_activeConnections > 0)
                    _activeConnections--;
            }

            _logger.LogDebug(
                "Connection closed. Active connections: {ActiveConnections}",
                _activeConnections);

            base.ConnectionClosed(connection, eventData);
        }

        public override async Task ConnectionClosedAsync(
            DbConnection connection,
            ConnectionEndEventData eventData)
        {
            lock (_lock)
            {
                if (_activeConnections > 0)
                    _activeConnections--;
            }

            _logger.LogDebug(
                "Connection closed. Active connections: {ActiveConnections}",
                _activeConnections);

            await base.ConnectionClosedAsync(connection, eventData);
        }

        public override void ConnectionFailed(
            DbConnection connection,
            ConnectionErrorEventData eventData)
        {
            lock (_lock)
            {
                _failedConnections++;
            }

            _logger.LogError(
                eventData.Exception,
                "Connection failed to database: {Database}. Failed attempts: {FailedConnections}",
                connection.Database,
                _failedConnections);

            base.ConnectionFailed(connection, eventData);
        }

        public override async Task ConnectionFailedAsync(
            DbConnection connection,
            ConnectionErrorEventData eventData,
            CancellationToken cancellationToken = default)
        {
            lock (_lock)
            {
                _failedConnections++;
            }

            _logger.LogError(
                eventData.Exception,
                "Connection failed to database: {Database}. Failed attempts: {FailedConnections}",
                connection.Database,
                _failedConnections);

            await base.ConnectionFailedAsync(connection, eventData, cancellationToken);
        }

        public void ResetCounters()
        {
            lock (_lock)
            {
                _activeConnections = 0;
                _failedConnections = 0;
            }
        }
    }
}