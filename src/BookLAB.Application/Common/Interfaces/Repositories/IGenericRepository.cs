namespace BookLAB.Application.Common.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> Entities { get; }
        Task<T?> GetByIdAsync(object id);
        T? GetById(object id);
        Task<List<T>> GetAllAsync();
        Task AddAsync(T entity);
        void AddRange(IEnumerable<T> entities);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
    }
}
