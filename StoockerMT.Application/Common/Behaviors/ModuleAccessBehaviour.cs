using MediatR;
using StoockerMT.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Domain.Exceptions.Module;
using StoockerMT.Domain.Repositories.UnitOfWork;

namespace StoockerMT.Application.Common.Behaviors
{
    public interface IModuleRequired
    {
        string ModuleCode { get; }
    }

    public class ModuleAccessBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IModuleRequired
    {
        private readonly ICurrentTenantService _currentTenantService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMasterDbUnitOfWork _unitOfWork;

        public ModuleAccessBehaviour(
            ICurrentTenantService currentTenantService,
            ICurrentUserService currentUserService,
            IMasterDbUnitOfWork unitOfWork)
        {
            _currentTenantService = currentTenantService;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_currentTenantService.HasTenant || _currentTenantService.TenantId == null)
            {
                throw new UnauthorizedAccessException("No tenant context found");
            }

            var module = await _unitOfWork.Modules.GetByCodeAsync(request.ModuleCode, cancellationToken);
            if (module == null)
            {
                throw new ModuleNotFoundException(0); // Module not found by code
            }

            var hasSubscription = await _unitOfWork.Subscriptions
                .HasActiveSubscriptionAsync(_currentTenantService.TenantId.Value, module.Id, cancellationToken);

            if (!hasSubscription)
            {
                throw new ModuleNotSubscribedException(_currentTenantService.TenantId.Value, module.Id);
            }

            // TODO: Check user permissions for the module

            return await next();
        }
    }
}
