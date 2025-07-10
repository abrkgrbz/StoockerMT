using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Domain.Enums;
using StoockerMT.Domain.Specifications;

namespace StoockerMT.Application.Common.Specifications.MasterDb.Tenant
{
     public class TenantSearchSpecification : BaseSpecification<Domain.Entities.MasterDb.Tenant>
    {
        public TenantSearchSpecification(
            string searchTerm = null,
            TenantStatus? status = null,
            int? pageNumber = null,
            int? pageSize = null)
        {
            // Build criteria
            Criteria = t => !t.IsDeleted;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                Criteria = Criteria.And(t => 
                    t.Name.Contains(searchTerm) || 
                    t.Code.Value.Contains(searchTerm) ||
                    t.Description.Contains(searchTerm));
            }

            if (status.HasValue)
            {
                Criteria = Criteria.And(t => t.Status == status.Value);
            }
             
            ApplyOrderByDescending(t => t.CreatedAt);
             
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                ApplyPaging((pageNumber.Value - 1) * pageSize.Value, pageSize.Value);
            }
             
            ApplyNoTracking();
        }
    }
}
