using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using StoockerMT.Application.Common.Interfaces;
using StoockerMT.Domain.Entities.Common;
using StoockerMT.Domain.Entities.TenantDb.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Persistence.Interceptors
{
    public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;

        public AuditableEntitySaveChangesInterceptor(
            ICurrentUserService currentUserService,
            IDateTime dateTime)
        {
            _currentUserService = currentUserService;
            _dateTime = dateTime;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public void UpdateEntities(DbContext? context)
        {
            if (context == null) return;

            var now = _dateTime.UtcNow;
            var userId = _currentUserService.UserId ?? "System";

            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity is BaseEntity baseEntity)
                    {
                        baseEntity.SetCreatedAt(now);
                    }

                    if (entry.Entity is TenantBaseEntity tenantBaseEntity)
                    {
                        tenantBaseEntity.SetCreatedAt(now);
                        tenantBaseEntity.CreatedBy = userId;
                    }

                    if (entry.Entity is IAuditableEntity auditableEntity)
                    {
                        auditableEntity.CreatedAt = now;
                        auditableEntity.CreatedBy = userId;
                    }
                }

                if (entry.State == EntityState.Modified)
                {
                    if (entry.Entity is BaseEntity baseEntity)
                    {
                        baseEntity.UpdateTimestamp();
                    }

                    if (entry.Entity is TenantBaseEntity tenantBaseEntity)
                    {
                        tenantBaseEntity.UpdateTimestamp();
                        tenantBaseEntity.UpdatedBy = userId;
                    }

                    if (entry.Entity is IAuditableEntity auditableEntity)
                    {
                        auditableEntity.UpdatedAt = now;
                        auditableEntity.UpdatedBy = userId;
                    }
                }
            }
        }
    }
}
