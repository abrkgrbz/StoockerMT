using StoockerMT.Domain.Entities.Common;
using StoockerMT.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Specifications
{
    public abstract class BaseSpecification<T> : Specification<T> where T : class
    {
        protected BaseSpecification() { }

        protected BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }
    }

    public class GetByIdSpecification<T> : BaseSpecification<T> where T : BaseEntity
    {
        public GetByIdSpecification(int id) : base(x => x.Id == id)
        {
            ApplyNoTracking();
        }
    }

    public class ActiveEntitiesSpecification<T> : BaseSpecification<T> where T : BaseEntity
    {
        public ActiveEntitiesSpecification() : base(x => !x.IsDeleted)
        {
        }
    }

    public class PaginatedSpecification<T> : BaseSpecification<T> where T : class
    {
        public PaginatedSpecification(int pageNumber, int pageSize)
        {
            ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        }
    }

    public class DateRangeSpecification<T> : BaseSpecification<T> where T : class,IAuditableEntity
    {
        public DateRangeSpecification(DateTime startDate, DateTime endDate)
            : base(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate)
        {
        }
    }
    public class SearchByNameSpecification<T> : BaseSpecification<T> where T : class
    {
        public SearchByNameSpecification(string searchTerm, Expression<Func<T, string>> nameProperty)
        {
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var property = Expression.Invoke(nameProperty, parameter);
                var contains = Expression.Call(property, "Contains", null, Expression.Constant(searchTerm));
                Criteria = Expression.Lambda<Func<T, bool>>(contains, parameter);
            }
        }
    }
     
    public class OrderedSpecification<T> : BaseSpecification<T> where T : class
    {
        public OrderedSpecification(Expression<Func<T, object>> orderBy, bool descending = false)
        {
            if (descending)
                ApplyOrderByDescending(orderBy);
            else
                ApplyOrderBy(orderBy);
        }
    }
     
    public class ActivePaginatedSpecification<T> : BaseSpecification<T> where T : BaseEntity
    {
        public ActivePaginatedSpecification(int pageNumber, int pageSize)
            : base(x => !x.IsDeleted)
        {
            ApplyPaging((pageNumber - 1) * pageSize, pageSize);
            ApplyOrderByDescending(x => x.CreatedAt);
        }
    }
}
