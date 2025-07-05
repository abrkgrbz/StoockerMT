using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using StoockerMT.Application.Common.Interfaces;

namespace StoockerMT.Application.Common.Behaviors
{
    public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly Stopwatch _timer;
        private readonly ILogger<TRequest> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICurrentTenantService _currentTenantService;

        public PerformanceBehaviour(
            ILogger<TRequest> logger,
            ICurrentUserService currentUserService,
            ICurrentTenantService currentTenantService)
        {
            _timer = new Stopwatch();
            _logger = logger;
            _currentUserService = currentUserService;
            _currentTenantService = currentTenantService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _timer.Start();

            var response = await next();

            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;

            if (elapsedMilliseconds > 500) // Log slow requests
            {
                var requestName = typeof(TRequest).Name;
                var userId = _currentUserService.UserId ?? string.Empty;
                var userName = _currentUserService.UserName ?? string.Empty;
                var tenantId = _currentTenantService.TenantId?.ToString() ?? string.Empty;

                _logger.LogWarning("Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName} {@TenantId} {@Request}",
                    requestName, elapsedMilliseconds, userId, userName, tenantId, request);
            }

            return response;
        }
    }
}
