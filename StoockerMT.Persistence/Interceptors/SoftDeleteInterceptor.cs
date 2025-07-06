using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using StoockerMT.Domain.Entities.Common;
using StoockerMT.Domain.Entities.TenantDb.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Persistence.Interceptors
{
    public class SoftDeleteInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            HandleSoftDelete(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            HandleSoftDelete(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void HandleSoftDelete(DbContext? context)
        {
            if (context == null) return;

            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Deleted)
                {
                    if (entry.Entity is BaseEntity baseEntity)
                    {
                        entry.State = EntityState.Modified;
                        baseEntity.MarkAsDeleted();
                    }
                    else if (entry.Entity is TenantBaseEntity tenantBaseEntity)
                    {
                        entry.State = EntityState.Modified;
                        tenantBaseEntity.MarkAsDeleted();
                    }
                }
            }
        }
    }
}
