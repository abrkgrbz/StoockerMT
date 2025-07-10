using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace StoockerMT.Application.Common.Interfaces.Services
{

    public interface IResilientDatabaseService
    {
        Task<T> ExecuteAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default);
        Task ExecuteAsync(Func<Task> operation, CancellationToken cancellationToken = default);

        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default);

        Task<bool> TestConnectionAsync(string connectionString, CancellationToken cancellationToken = default);
        Task<bool> IsDatabaseHealthyAsync(DbContext context, CancellationToken cancellationToken = default);
    }
}
