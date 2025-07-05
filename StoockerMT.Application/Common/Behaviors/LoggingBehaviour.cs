using MediatR;
using Microsoft.Extensions.Logging;
using StoockerMT.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Behaviors
{
    public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICurrentTenantService _currentTenantService;

        public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger,
            ICurrentUserService currentUserService,
            ICurrentTenantService currentTenantService)
        {
            _logger = logger;
            _currentUserService = currentUserService;
            _currentTenantService = currentTenantService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _currentUserService.UserId ?? "Anonymous";
            var userName = _currentUserService.UserName ?? "Anonymous";
            var tenantId = _currentTenantService.TenantId?.ToString() ?? "No Tenant";

            _logger.LogInformation("Handling {RequestName} for User {UserId} ({UserName}) in Tenant {TenantId}",
                requestName, userId, userName, tenantId);

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                var response = await next();

                stopwatch.Stop();

                _logger.LogInformation("Handled {RequestName} in {ElapsedMilliseconds}ms",
                    requestName, stopwatch.ElapsedMilliseconds);

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(ex, "Error handling {RequestName} after {ElapsedMilliseconds}ms",
                    requestName, stopwatch.ElapsedMilliseconds);

                throw;
            }
        }
    }
}
