using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, string orderByMember, bool isAscending)
        {
            var entityType = typeof(T);
            var propertyInfo = entityType.GetProperty(orderByMember);

            if (propertyInfo == null)
                return query;

            var arg = Expression.Parameter(entityType, "x");
            var property = Expression.Property(arg, orderByMember);
            var selector = Expression.Lambda(property, arg);

            var method = isAscending ? "OrderBy" : "OrderByDescending";
            var genericMethod = typeof(Queryable)
                .GetMethods()
                .Where(m => m.Name == method && m.GetParameters().Length == 2)
                .Single()
                .MakeGenericMethod(entityType, propertyInfo.PropertyType);

            return (IQueryable<T>)genericMethod.Invoke(null, new object[] { query, selector })!;
        }

        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
        {
            return condition ? query.Where(predicate) : query;
        }
    }
}
