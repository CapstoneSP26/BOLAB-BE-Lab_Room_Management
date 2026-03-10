////using BookLAB.Application.Common.Interfaces.Persistence;
//using BookLAB.Application.Common.Interfaces.Repositories;
////using BookLAB.Infrastructure.Persistence.Repositories;
//using BookLAB.Infrastructure.Repositories;
//using Microsoft.EntityFrameworkCore.Storage;
//using System.Collections;

//namespace BookLAB.Infrastructure.Persistence;

///// <summary>
///// Implements the Unit of Work pattern to coordinate repositories 
///// and ensure atomic transactions.
///// </summary>
//public class UnitOfWork : IUnitOfWork
//{
//    private readonly BookLABDbContext _context; // Using concrete class for full EF Core features
//    private IDbContextTransaction? _currentTransaction;
//    private Hashtable? _repositories;

//    // Explicit repository for specialized Booking logic
//    private IBookingRepository? _bookingRepository;
//    private ILabOwnerRepository? _labOwnerRepository;

//    public UnitOfWork(BookLABDbContext context)
//    {
//        _context = context;
//    }

//    /// <summary>
//    /// Specialized repository for Booking with custom business logic (e.g., IsOverlappedAsync)
//    /// </summary>
//    public IBookingRepository Bookings => _bookingRepository ??= new BookingRepository(_context);
//    public ILabOwnerRepository LabOwners => _labOwnerRepository ??= new LabOwnerRepository(_context);

//    /// <summary>
//    /// Generic repository factory. Creates and caches repositories for any entity on the fly.
//    /// </summary>
//    public IGenericRepository<T> Repository<T>() where T : class
//    {
//        _repositories ??= new Hashtable();

//        var type = typeof(T).Name;

//        if (!_repositories.ContainsKey(type))
//        {
//            var repositoryType = typeof(GenericRepository<>);
//            var repositoryInstance = Activator.CreateInstance(
//                repositoryType.MakeGenericType(typeof(T)), _context);

//            _repositories.Add(type, repositoryInstance!);
//        }

//        return (IGenericRepository<T>)_repositories[type]!;
//    }

//    /// <summary>
//    /// Saves all changes tracked by the DbContext to the database.
//    /// </summary>
//    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
//    {
//        return await _context.SaveChangesAsync(cancellationToken);
//    }

//    #region Transaction Management

//    /// <summary>
//    /// Starts a new database transaction.
//    /// </summary>
//    public async Task BeginTransactionAsync()
//    {
//        if (_currentTransaction != null) return;
//        _currentTransaction = await _context.Database.BeginTransactionAsync();
//    }

//    /// <summary>
//    /// Commits the current transaction and flushes all changes.
//    /// </summary>
//    public async Task CommitTransactionAsync()
//    {
//        try
//        {
//            await _context.SaveChangesAsync();
//            if (_currentTransaction != null)
//            {
//                await _currentTransaction.CommitAsync();
//            }
//        }
//        catch
//        {
//            await RollbackTransactionAsync();
//            throw;
//        }
//        finally
//        {
//            DisposeTransaction();
//        }
//    }

//    /// <summary>
//    /// Discards all changes in the current transaction.
//    /// </summary>
//    public async Task RollbackTransactionAsync()
//    {
//        try
//        {
//            if (_currentTransaction != null)
//            {
//                await _currentTransaction.RollbackAsync();
//            }
//        }
//        finally
//        {
//            DisposeTransaction();
//        }
//    }

//    private void DisposeTransaction()
//    {
//        _currentTransaction?.Dispose();
//        _currentTransaction = null;
//    }

//    #endregion

//    public void Dispose()
//    {
//        _context.Dispose();
//        GC.SuppressFinalize(this);
//    }
//}