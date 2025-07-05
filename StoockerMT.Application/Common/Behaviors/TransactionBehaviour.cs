using MediatR;
using Microsoft.Extensions.Logging;
using StoockerMT.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Behaviors
{
    public interface ITransactional { }

    public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ITransactional
    {
        private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> _logger;
        private readonly IApplicationDbContext _dbContext;

        public TransactionBehaviour(ILogger<TransactionBehaviour<TRequest, TResponse>> logger, IApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = default(TResponse);
            var typeName = typeof(TRequest).Name;

            try
            {
                if (_dbContext.HasActiveTransaction)
                {
                    return await next();
                }

                var strategy = _dbContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    using var transaction = await _dbContext.BeginTransactionAsync(cancellationToken);

                    _logger.LogInformation("----- Begin transaction {TransactionId} for {CommandName}",
                        transaction.TransactionId, typeName);

                    response = await next();

                    await _dbContext.CommitTransactionAsync(transaction, cancellationToken);

                    _logger.LogInformation("----- Commit transaction {TransactionId} for {CommandName}",
                        transaction.TransactionId, typeName);
                });

                return response!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR Handling transaction for {CommandName}", typeName);
                throw;
            }
        }
    }
}
