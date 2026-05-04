using BookLAB.Application.Common.Specifications;

namespace BookLAB.Application.Common.Extensions
{
    public static class SpecificationExtensions
    {
        public static IQueryable<TEntity> ApplySpecification<TEntity>(this IQueryable<TEntity> query, BaseSpecification<TEntity> spec)
            where TEntity : class
        {
            return SpecificationEvaluator<TEntity>.GetQuery(query, spec);
        }
    }
}
