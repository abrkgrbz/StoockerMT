using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Domain.Enums;
using StoockerMT.Domain.Specifications;

namespace StoockerMT.Application.Common.Specifications.TenantDb.Customer
{
    public class CustomersByLocationSpecification : BaseSpecification<Domain.Entities.TenantDb.Customer>
    {
        public CustomersByLocationSpecification(string country, string city = null)
        {
            Criteria = c => c.Address != null && c.Address.Country == country;

            if (!string.IsNullOrWhiteSpace(city))
            {
                Criteria = Criteria.And(c => c.Address.City == city);
            }

            ApplyOrderBy(c => c.CustomerName);
        }
    }
     
 
}
