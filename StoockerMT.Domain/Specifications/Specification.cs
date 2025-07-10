using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace StoockerMT.Domain.Specifications
{
    public abstract class Specification<T>
    {
        protected Specification() { }

        // Core specification
        public Expression<Func<T, bool>> Criteria { get; protected set; }

        // Include specifications
        public List<Expression<Func<T, object>>> Includes { get; } = new();
        public List<string> IncludeStrings { get; } = new();

        // Ordering specifications
        public Expression<Func<T, object>> OrderBy { get; private set; }
        public Expression<Func<T, object>> OrderByDescending { get; private set; }
        public List<Expression<Func<T, object>>> ThenByList { get; } = new();
        public List<Expression<Func<T, object>>> ThenByDescendingList { get; } = new();

        // Paging specifications
        public int Take { get; private set; }
        public int Skip { get; private set; }
        public bool IsPagingEnabled { get; private set; }

        // Tracking specifications
        public bool AsNoTracking { get; private set; } = true;
        public bool AsSplitQuery { get; private set; }
        public bool AsNoTrackingWithIdentityResolution { get; private set; }

        // Caching
        public bool CacheEnabled { get; private set; }
        public string CacheKey { get; private set; }
        public TimeSpan? CacheDuration { get; private set; }

        // Methods for building specifications
        protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        protected virtual void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }

        protected virtual void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }

        protected virtual void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        protected virtual void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            OrderByDescending = orderByDescendingExpression;
        }

        protected virtual void ApplyThenBy(Expression<Func<T, object>> thenByExpression)
        {
            ThenByList.Add(thenByExpression);
        }

        protected virtual void ApplyThenByDescending(Expression<Func<T, object>> thenByDescendingExpression)
        {
            ThenByDescendingList.Add(thenByDescendingExpression);
        }

        protected virtual void ApplyNoTracking()
        {
            AsNoTracking = true;
        }

        protected virtual void ApplyTracking()
        {
            AsNoTracking = false;
        }

        protected virtual void ApplySplitQuery()
        {
            AsSplitQuery = true;
        }

        protected virtual void ApplyNoTrackingWithIdentityResolution()
        {
            AsNoTrackingWithIdentityResolution = true;
            AsNoTracking = false;
        }

        protected virtual void EnableCache(string cacheKey, TimeSpan? duration = null)
        {
            CacheEnabled = true;
            CacheKey = cacheKey;
            CacheDuration = duration ?? TimeSpan.FromMinutes(5);
        }

        // Specification composition
        public Specification<T> And(Specification<T> specification)
        {
            return new AndSpecification<T>(this, specification);
        }

        public Specification<T> Or(Specification<T> specification)
        {
            return new OrSpecification<T>(this, specification);
        }

        public Specification<T> Not()
        {
            return new NotSpecification<T>(this);
        }

        public virtual bool IsSatisfiedBy(T entity)
        {
            var predicate = Criteria?.Compile();
            return predicate?.Invoke(entity) ?? true;
        }
    }
     
    public sealed class AndSpecification<T> : Specification<T>
    {
        private readonly Specification<T> _left;
        private readonly Specification<T> _right;

        public AndSpecification(Specification<T> left, Specification<T> right)
        {
            _left = left;
            _right = right;

            // Combine criteria
            if (_left.Criteria != null && _right.Criteria != null)
            {
                Criteria = _left.Criteria.And(_right.Criteria);
            }
            else if (_left.Criteria != null)
            {
                Criteria = _left.Criteria;
            }
            else if (_right.Criteria != null)
            {
                Criteria = _right.Criteria;
            }

            // Combine includes
            Includes.AddRange(_left.Includes);
            Includes.AddRange(_right.Includes);
            IncludeStrings.AddRange(_left.IncludeStrings);
            IncludeStrings.AddRange(_right.IncludeStrings);
        }
    }

    public sealed class OrSpecification<T> : Specification<T>
    {
        private readonly Specification<T> _left;
        private readonly Specification<T> _right;

        public OrSpecification(Specification<T> left, Specification<T> right)
        {
            _left = left;
            _right = right;

            if (_left.Criteria != null && _right.Criteria != null)
            {
                Criteria = _left.Criteria.Or(_right.Criteria);
            }
            else if (_left.Criteria != null)
            {
                Criteria = _left.Criteria;
            }
            else if (_right.Criteria != null)
            {
                Criteria = _right.Criteria;
            }
        }
    }

    public sealed class NotSpecification<T> : Specification<T>
    {
        public NotSpecification(Specification<T> specification)
        {
            if (specification.Criteria != null)
            {
                Criteria = specification.Criteria.Not();
            }
        }
    }

    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> And<T>(
            this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            var parameter = Expression.Parameter(typeof(T));
            var combined = new ParameterReplacer(parameter).Visit(
                Expression.AndAlso(
                    first.Body,
                    second.Body));
            return Expression.Lambda<Func<T, bool>>(combined, parameter);
        }

        public static Expression<Func<T, bool>> Or<T>(
            this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            var parameter = Expression.Parameter(typeof(T));
            var combined = new ParameterReplacer(parameter).Visit(
                Expression.OrElse(
                    first.Body,
                    second.Body));
            return Expression.Lambda<Func<T, bool>>(combined, parameter);
        }

        public static Expression<Func<T, bool>> Not<T>(
            this Expression<Func<T, bool>> expression)
        {
            var parameter = Expression.Parameter(typeof(T));
            var not = Expression.Not(expression.Body);
            return Expression.Lambda<Func<T, bool>>(not, parameter);
        }

        private class ParameterReplacer : ExpressionVisitor
        {
            private readonly ParameterExpression _parameter;

            public ParameterReplacer(ParameterExpression parameter)
            {
                _parameter = parameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return _parameter;
            }
        }
    }
}