using System.Linq;
using Microsoft.EntityFrameworkCore;
using StoockerMT.Domain.Specifications;

namespace StoockerMT.Persistence.Specifications
{
    public static class SpecificationEvaluator<T> where T : class
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, Specification<T> specification)
        {
            var query = inputQuery;
             
            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }
             
            query = specification.Includes.Aggregate(query,
                (current, include) => current.Include(include));
             
            query = specification.IncludeStrings.Aggregate(query,
                (current, include) => current.Include(include));
             
            if (specification.OrderBy != null)
            {
                query = query.OrderBy(specification.OrderBy);

                // Apply ThenBy
                if (specification.ThenByList.Any())
                {
                    var orderedQuery = (IOrderedQueryable<T>)query;
                    query = specification.ThenByList.Aggregate(orderedQuery,
                        (current, thenBy) => current.ThenBy(thenBy));
                }
            }
            else if (specification.OrderByDescending != null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);

                // Apply ThenByDescending
                if (specification.ThenByDescendingList.Any())
                {
                    var orderedQuery = (IOrderedQueryable<T>)query;
                    query = specification.ThenByDescendingList.Aggregate(orderedQuery,
                        (current, thenBy) => current.ThenByDescending(thenBy));
                }
            }
             
            if (specification.IsPagingEnabled)
            {
                query = query.Skip(specification.Skip).Take(specification.Take);
            }
             
            if (specification.AsNoTracking)
            {
                query = query.AsNoTracking();
            }
            else if (specification.AsNoTrackingWithIdentityResolution)
            {
                query = query.AsNoTrackingWithIdentityResolution();
            }
             
            if (specification.AsSplitQuery)
            {
                query = query.AsSplitQuery();
            }

            return query;
        }
    }
}