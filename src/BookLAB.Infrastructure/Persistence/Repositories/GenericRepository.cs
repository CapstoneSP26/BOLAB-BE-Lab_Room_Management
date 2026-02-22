using BookLAB.Application.Common.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class 
    {
        private readonly BookLABDbContext _context;
        public GenericRepository(BookLABDbContext context) => _context = context;

        public IQueryable<T> Entities => _context.Set<T>().AsNoTracking();

        public async Task<T?> GetByIdAsync(object id) => await _context.Set<T>().FindAsync(id);

        public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);

        public void Update(T entity) => _context.Set<T>().Update(entity);

        public void Delete(T entity) => _context.Set<T>().Remove(entity);

        public async Task<List<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();
    }
}
