using Microsoft.Extensions.Caching.Memory;
using OrderService.Data.Interfaces;
using OrderService.Entities;

namespace OrderService.Data.Repositories;

public class CacheProductRepository : IProductRepository
{
  private readonly IProductRepository _underlyingProductRepository;
  private readonly IMemoryCache _cache;

  public CacheProductRepository(IProductRepository underlyingProductRepository, IMemoryCache cache)
  {
    _underlyingProductRepository = underlyingProductRepository;
    _cache = cache;
  }

  public async Task<Product?> GetProductAsync(string productName, CancellationToken cancellationToken)
  {
    string cacheKey = $"product_{productName}";

    if (!_cache.TryGetValue(cacheKey, out Product? product))
    {
      product = await _underlyingProductRepository.GetProductAsync(productName, cancellationToken);

      if (product != null)
      {
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(5)) // Keep in cache if accessed every 5 mins
            .SetAbsoluteExpiration(TimeSpan.FromHours(1)) // Remove after 1 hour maximum
            .SetPriority(CacheItemPriority.Normal);

        _cache.Set(cacheKey, product, cacheEntryOptions);
      }
    }

    return product;
  }
}


