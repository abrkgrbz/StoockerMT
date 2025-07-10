using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Domain.Enums;

namespace StoockerMT.Application.Common.Specifications.MasterDb.Tenant
{
    public class TenantsNeedingBackupSpecification : BaseSpecification<Domain.Entities.MasterDb.Tenant>
    {
        public TenantsNeedingBackupSpecification(int backupIntervalDays = 1)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-backupIntervalDays);

            Criteria = t => t.Status == TenantStatus.Active &&
                            t.DatabaseInfo != null &&
                            (t.DatabaseInfo.LastBackupDate == null ||
                             t.DatabaseInfo.LastBackupDate < cutoffDate);
        }
    }
}
