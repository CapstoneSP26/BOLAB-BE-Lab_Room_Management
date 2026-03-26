using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Common.Specifications
{
    public class SpecificationEvaluator<TEntity> where TEntity : class
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, BaseSpecification<TEntity> spec)
        {
            var query = inputQuery;
            // Áp dụng filter (Where)
            if(spec.Criteria != null)
            {
                foreach (var criterion in spec.Criteria)
                {
                    query = query.Where(criterion);
                }
            }

            // Áp dụng sắp xếp (OrderBy, OrderByDescending)
            if (spec.OrderBy != null)
            {
                query = query.OrderBy(spec.OrderBy);
            }
            else if (spec.OrderByDescending != null)
            {
                query = query.OrderByDescending(spec.OrderByDescending);
            }

            // Áp dụng các liên kết bảng (Includes)
            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

            return query;
        }
    }
}
