using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using StoockerMT.Domain.Repositories.UnitOfWork;

namespace StoockerMT.Persistence.Repositories.Common
{
    public abstract class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;
        private IDbContextTransaction? _currentTransaction;
        private bool _disposed;

        protected UnitOfWork(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null)
            {
                return;
            }

            if (_context.Database.CurrentTransaction != null)
            {
                _currentTransaction = _context.Database.CurrentTransaction;
                return;
            }

            // Check if retry strategy is enabled
            var executionStrategy = _context.Database.CreateExecutionStrategy();
            if (executionStrategy.RetriesOnFailure)
            {
                throw new InvalidOperationException(
                    "Cannot begin a transaction when retry execution strategy is enabled. " +
                    "Use ExecuteInTransactionAsync method instead.");
            }

            _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public virtual async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction == null) return;

            try
            {
                await SaveChangesAsync(cancellationToken);

                if (_context.Database.CurrentTransaction?.TransactionId == _currentTransaction.TransactionId)
                {
                    await _currentTransaction.CommitAsync(cancellationToken);
                }
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public virtual async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction == null) return;

            try
            {
                if (_context.Database.CurrentTransaction?.TransactionId == _currentTransaction.TransactionId)
                {
                    await _currentTransaction.RollbackAsync(cancellationToken);
                }
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
         
        public virtual async Task<T> ExecuteInTransactionAsync<T>(
            Func<CancellationToken, Task<T>> operation,
            CancellationToken cancellationToken = default)
        { 
            var strategy = _context.Database.CreateExecutionStrategy();
             
            return await strategy.ExecuteAsync(async (ct) =>
            { 
                await using var transaction = await _context.Database.BeginTransactionAsync(ct);

                try
                { 
                    var result = await operation(ct);
                     
                    await _context.SaveChangesAsync(ct);
                     
                    await transaction.CommitAsync(ct);

                    return result;
                }
                catch (Exception ex)
                { 
                    await transaction.RollbackAsync(ct);
                    throw;
                }
            }, cancellationToken);
        }
         
        public virtual async Task ExecuteInTransactionAsync(
            Func<CancellationToken, Task> operation,
            CancellationToken cancellationToken = default)
        { 
            await ExecuteInTransactionAsync(async (ct) =>
            {
                await operation(ct);
                return Task.FromResult(0);  
            }, cancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _currentTransaction?.Dispose();
                    _context?.Dispose();
                }
                _disposed = true;
            }
        }
    }
}