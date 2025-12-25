using BookLAB.Application.Common.Interfaces.Persistence;

namespace BookLAB.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IBookLABDbContext _context;

        public UnitOfWork(IBookLABDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
