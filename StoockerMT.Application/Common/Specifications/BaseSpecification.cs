using StoockerMT.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Specifications
{
    public abstract class BaseSpecification<T> : Specification<T>
    {
        protected BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        public Expression<Func<T, bool>> Criteria { get; }
        public List<Expression<Func<T, object>>> Includes { get; } = new();
        public List<string> IncludeStrings { get; } = new();

        protected void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        protected void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }

        public override Expression<Func<T, bool>> ToExpression()
        {
            return Criteria;
        }
    }
}
