using StoockerMT.Domain.Enums;
using StoockerMT.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Specifications.TenantDb.Customer
{
    public class CustomerSearchSpecification : BaseSpecification<Domain.Entities.TenantDb.Customer>
    {
        public CustomerSearchSpecification(
            string searchTerm = null,
            CustomerType? type = null,
            CustomerStatus? status = null,
            bool includeContacts = false,
            bool includeOrders = false,
            int? pageNumber = null,
            int? pageSize = null)
        {
            // Base criteria - active records
            Criteria = c => !c.IsDeleted;

            // Search criteria
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                Criteria = Criteria.And(c =>
                    c.CustomerName.ToLower().Contains(searchTerm) ||
                    c.CustomerCode.ToLower().Contains(searchTerm) ||
                    (c.ContactPerson != null && c.ContactPerson.ToLower().Contains(searchTerm)) ||
                    (c.Email != null && c.Email.Value.ToLower().Contains(searchTerm)));
            }

            // Type filter
            if (type.HasValue)
            {
                Criteria = Criteria.And(c => c.Type == type.Value);
            }

            // Status filter
            if (status.HasValue)
            {
                Criteria = Criteria.And(c => c.Status == status.Value);
            }

            // Includes
            if (includeContacts)
            {
                AddInclude(c => c.Contacts);
            }

            if (includeOrders)
            {
                AddInclude(c => c.Orders);
                AddInclude("Orders.Items");
                ApplySplitQuery();
            }

            // Ordering
            ApplyOrderBy(c => c.CustomerName);

            // Paging
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                ApplyPaging((pageNumber.Value - 1) * pageSize.Value, pageSize.Value);
            }

            // No tracking for search operations
            ApplyNoTracking();
        }
    }
}
