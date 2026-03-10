//using BookLAB.Application.Common.Interfaces.Repositories;

//namespace BookLAB.Application.Common.Interfaces.Persistence
//{
//    public interface IUnitOfWork : IDisposable
//    {
//        // Common Repository 
//        IGenericRepository<T> Repository<T>() where T : class;

//        // Specialized Repository
//        IBookingRepository Bookings { get; }
//        ILabOwnerRepository LabOwners { get; }
//        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
//        Task BeginTransactionAsync();
//        Task CommitTransactionAsync();
//        Task RollbackTransactionAsync();
//    }
//}
