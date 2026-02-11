using Application.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace Application.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public bool TryGet<T>(string key, out T value)
        {
            return _memoryCache.TryGetValue(key, out value);
        }

        public void Set<T>(string key, T value, TimeSpan? slidingExpiration = null)
        {
            var options = new MemoryCacheEntryOptions();

            options.SetSlidingExpiration(slidingExpiration ?? TimeSpan.FromMinutes(30));

            _memoryCache.Set(key, value, options);
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }
    }
}