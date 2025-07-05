﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using StoockerMT.Domain.Entities.Common;
using StoockerMT.Domain.Entities.TenantDb.Common;

namespace StoockerMT.Domain.Specifications
{
    public abstract class Specification<T>
    {
        public abstract Expression<Func<T, bool>> ToExpression();

        public bool IsSatisfiedBy(T entity)
        {
            var predicate = ToExpression().Compile();
            return predicate(entity);
        }

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

        public class AndSpecification<T> : Specification<T>
        {
            private readonly Specification<T> _left;
            private readonly Specification<T> _right;

            public AndSpecification(Specification<T> left, Specification<T> right)
            {
                _left = left;
                _right = right;
            }

            public override Expression<Func<T, bool>> ToExpression()
            {
                var leftExpression = _left.ToExpression();
                var rightExpression = _right.ToExpression();

                var parameter = Expression.Parameter(typeof(T));
                var body = Expression.AndAlso(
                    Expression.Invoke(leftExpression, parameter),
                    Expression.Invoke(rightExpression, parameter)
                );

                return Expression.Lambda<Func<T, bool>>(body, parameter);
            }
        }

        public class OrSpecification<T> : Specification<T>
        {
            private readonly Specification<T> _left;
            private readonly Specification<T> _right;

            public OrSpecification(Specification<T> left, Specification<T> right)
            {
                _left = left;
                _right = right;
            }

            public override Expression<Func<T, bool>> ToExpression()
            {
                var leftExpression = _left.ToExpression();
                var rightExpression = _right.ToExpression();

                var parameter = Expression.Parameter(typeof(T));
                var body = Expression.OrElse(
                    Expression.Invoke(leftExpression, parameter),
                    Expression.Invoke(rightExpression, parameter)
                );

                return Expression.Lambda<Func<T, bool>>(body, parameter);
            }
        }

        public class NotSpecification<T> : Specification<T>
        {
            private readonly Specification<T> _specification;

            public NotSpecification(Specification<T> specification)
            {
                _specification = specification;
            }

            public override Expression<Func<T, bool>> ToExpression()
            {
                var expression = _specification.ToExpression();
                var parameter = Expression.Parameter(typeof(T));
                var body = Expression.Not(Expression.Invoke(expression, parameter));

                return Expression.Lambda<Func<T, bool>>(body, parameter);
            }
        }
    }
}
