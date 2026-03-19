using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Common.Specifications
{
    public class SpecificationEvaluator<TEntity> where TEntity : class
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, BaseSpecification<TEntity> spec)
        {
            var query = inputQuery;

            // Áp dụng filter (Where)
            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }

            // Áp dụng các liên kết bảng (Includes)
            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

            return query;
        }
    }
}
