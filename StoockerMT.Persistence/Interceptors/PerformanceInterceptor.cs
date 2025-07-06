using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Persistence.Interceptors
{
    public class PerformanceInterceptor : DbCommandInterceptor
    {
        private readonly ILogger<PerformanceInterceptor> _logger;
        private readonly Stopwatch _stopwatch = new();

        public PerformanceInterceptor(ILogger<PerformanceInterceptor> logger)
        {
            _logger = logger;
        }

        public override InterceptionResult<DbDataReader> ReaderExecuting(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result)
        {
            _stopwatch.Restart();
            return base.ReaderExecuting(command, eventData, result);
        }

        public override DbDataReader ReaderExecuted(
            DbCommand command,
            CommandExecutedEventData eventData,
            DbDataReader result)
        {
            _stopwatch.Stop();

            if (_stopwatch.ElapsedMilliseconds > 500)
            {
                var commandText = command.CommandText.Length > 100
                    ? command.CommandText.Substring(0, 100) + "..."
                    : command.CommandText;

                _logger.LogWarning(
                    "Slow query ({ElapsedMilliseconds}ms): {CommandText}",
                    _stopwatch.ElapsedMilliseconds,
                    commandText);
            }

            return base.ReaderExecuted(command, eventData, result);
        }

        public override async ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = default)
        {
            _stopwatch.Restart();
            return await base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override async ValueTask<DbDataReader> ReaderExecutedAsync(
            DbCommand command,
            CommandExecutedEventData eventData,
            DbDataReader result,
            CancellationToken cancellationToken = default)
        {
            _stopwatch.Stop();

            if (_stopwatch.ElapsedMilliseconds > 500)
            {
                var commandText = command.CommandText.Length > 100
                    ? command.CommandText.Substring(0, 100) + "..."
                    : command.CommandText;

                _logger.LogWarning(
                    "Slow query ({ElapsedMilliseconds}ms): {CommandText}",
                    _stopwatch.ElapsedMilliseconds,
                    commandText);
            }

            return await base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
        }
    }
}
