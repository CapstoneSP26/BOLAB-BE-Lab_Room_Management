using System.Linq.Expressions;

namespace BookLAB.Application.Common.Specifications
{
    public abstract class BaseSpecification<T>
    {
        public List<Expression<Func<T, bool>>> Criteria { get; protected set; } = new();
        public List<Expression<Func<T, object>>> Includes { get; } = new();
        public Expression<Func<T, object>>? OrderBy { get; protected set; }
        public Expression<Func<T, object>>? OrderByDescending { get; protected set; }

        protected void AddCriteria(Expression<Func<T, bool>> criterionExpression)
        {
            Criteria.Add(criterionExpression);
        }
        protected void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        protected void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            // Implementation for ordering (if needed)
            OrderBy = orderByExpression;
        }
        protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            // Implementation for ordering (if needed)
            OrderByDescending = orderByDescendingExpression;
        }
    }
}
