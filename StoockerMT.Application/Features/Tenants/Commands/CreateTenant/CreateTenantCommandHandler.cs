using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using StoockerMT.Application.Common.Interfaces.Services;
using StoockerMT.Application.Common.Models;
using StoockerMT.Domain.Entities.MasterDb;
using StoockerMT.Domain.Repositories.MasterDb;
using StoockerMT.Domain.Repositories.UnitOfWork;
using StoockerMT.Domain.ValueObjects;

namespace StoockerMT.Application.Features.Tenants.Commands.CreateTenant
{
    public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, Result<CreateTenantResponseData>>
    {
        private readonly IMasterDbUnitOfWork _unitOfWork;
        private readonly ITenantService _tenantService;
        private readonly ILogger<CreateTenantCommandHandler> _logger;
        private readonly IEncryptionService _encryptionService;
        public CreateTenantCommandHandler(IMasterDbUnitOfWork unitOfWork, ITenantService tenantService, ILogger<CreateTenantCommandHandler> logger, IEncryptionService encryptionService)
        {
            _unitOfWork = unitOfWork;
            _tenantService = tenantService;
            _logger = logger;
            _encryptionService = encryptionService;
        }

        public async Task<Result<CreateTenantResponseData>> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                var tenantCode = new TenantCode(request.Code);
                if (await _unitOfWork.Tenants.ExistsByCodeAsync(tenantCode, cancellationToken))
                {
                    return Result<CreateTenantResponseData>.Failure("Tenant code already exists", "A tenant with this code already exists");

                }
                var tenant = new Tenant(
                    name: request.Name,
                    code: tenantCode,
                    createdBy: "System",
                    description: request.Description
                );

                // Add tenant
                await _unitOfWork.Tenants.AddAsync(tenant, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Tenant created with ID: {TenantId}, Code: {TenantCode}",
                    tenant.Id, tenant.Code.Value);

                // Create tenant database
                string connectionString;
                try
                {
                    connectionString = await _tenantService.CreateTenantDatabaseAsync(
                        tenant.Id,
                        tenantCode.Value,
                        cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to create database for tenant {TenantId}", tenant.Id);
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);

                    return Result<CreateTenantResponseData>.Failure("Failed to create tenant database", ex.Message);


                }

                var databaseInfo = DatabaseInfo.Create(
                    databaseName: $"StoockerMT_Tenant_{tenantCode.Value}",
                    server: "localhost",
                    username: "sa",
                    encryptedPassword: _encryptionService.Encrypt(_encryptionService.GenerateSecurePassword(16)),
                    encryptedConnectionString: connectionString
                );

                tenant.SetDatabaseInfo(databaseInfo);

                // Activate tenant
                tenant.Activate("System");

                // Subscribe to selected modules
                if (request.SelectedModules?.Any() == true)
                {
                    var modules = await _unitOfWork.Modules.GetByCodesAsync(
                        request.SelectedModules,
                        cancellationToken);

                    foreach (var module in modules)
                    {

                        await _unitOfWork.Subscriptions.AddAsync(
                            new TenantModuleSubscription(tenant.Id, module.Id, request.SubscriptionType,
                                request.SubscriptionPrice, "System"), cancellationToken);

                    }
                }

                // Create admin user
                await _unitOfWork.TenantUsers.AddAsync(new TenantUser(tenant.Id, request.AdminFirstName,
                    request.AdminLastName, request.AdminEmail, _encryptionService.HashPassword(request.AdminPassword), "System"));
                     
                // Save all changes
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Commit transaction
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Tenant setup completed for {TenantCode}", tenantCode.Value);
                var data = new CreateTenantResponseData()
                {
                    Success = true,
                    TenantId = tenant.Id,
                    TenantCode = tenantCode.Value,
                    DatabaseName = databaseInfo.DatabaseName,
                    Message = "Tenant created successfully"
                };
                return Result<CreateTenantResponseData>.Success(data);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tenant");
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<CreateTenantResponseData>.Failure("An error occurred while creating the tenant", ex.Message);

            }
        }
    }

}
