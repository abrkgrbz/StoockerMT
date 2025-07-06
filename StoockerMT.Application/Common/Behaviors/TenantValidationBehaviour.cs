using MediatR;
using StoockerMT.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Domain.Exceptions.Tenant;

namespace StoockerMT.Application.Common.Behaviors
{
    public interface ITenantRequired { }

    public class TenantValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ITenantRequired
    {
        private readonly ICurrentTenantService _currentTenantService;

        public TenantValidationBehaviour(ICurrentTenantService currentTenantService)
        {
            _currentTenantService = currentTenantService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_currentTenantService.HasTenant || _currentTenantService.TenantId == null)
            {
                throw new TenantNotFoundException("No tenant context found");
            }

            return await next();
        }
    }
}
