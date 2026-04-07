using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class 
    {
        private readonly BookLABDbContext _context;
        public GenericRepository(BookLABDbContext context) => _context = context;

        public IQueryable<T> Entities => _context.Set<T>().AsNoTracking();

        public async Task<T?> GetByIdAsync(object id) => await _context.Set<T>().FindAsync(id);

        public T? GetById(object id) => _context.Set<T>().Find(id);

        public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);
        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }
        public void AddRange(IEnumerable<T> entities)
        {
            _context.Set<T>().AddRange(entities);
        }
        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
        }
        public void Update(T entity) => _context.Set<T>().Update(entity);

        public void Delete(T entity) => _context.Set<T>().Remove(entity);
        public void DeleteRange(IEnumerable<T> entities) => _context.Set<T>().RemoveRange(entities);

        public async Task<List<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();
    }
}
