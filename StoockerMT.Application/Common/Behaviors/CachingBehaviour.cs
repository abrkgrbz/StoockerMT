using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Behaviors
{
    public interface ICacheableQuery
    {
        string CacheKey { get; }
        TimeSpan? SlidingExpiration { get; }
        TimeSpan? AbsoluteExpiration { get; }
    }

    public class CachingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICacheableQuery
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<CachingBehaviour<TRequest, TResponse>> _logger;

        public CachingBehaviour(IDistributedCache cache, ILogger<CachingBehaviour<TRequest, TResponse>> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var cacheKey = request.CacheKey;
            var cachedResponse = await _cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrEmpty(cachedResponse))
            {
                _logger.LogInformation("Response retrieved from cache for {CacheKey}", cacheKey);
                return JsonSerializer.Deserialize<TResponse>(cachedResponse)!;
            }

            var response = await next();

            var options = new DistributedCacheEntryOptions();

            if (request.SlidingExpiration.HasValue)
                options.SetSlidingExpiration(request.SlidingExpiration.Value);

            if (request.AbsoluteExpiration.HasValue)
                options.SetAbsoluteExpiration(request.AbsoluteExpiration.Value);

            var serializedResponse = JsonSerializer.Serialize(response);
            await _cache.SetStringAsync(cacheKey, serializedResponse, options, cancellationToken);

            _logger.LogInformation("Response cached for {CacheKey}", cacheKey);

            return response;
        }
    }
}
