﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using SharedKernel.Application.Caching;
using SharedKernel.Application.Logging;

namespace SharedKernel.Infrastructure.Caching
{
    internal class InMemoryCacheHelper : ICacheHelper
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ICustomLogger<InMemoryCacheHelper> _customLogger;

        public InMemoryCacheHelper(IMemoryCache memoryCache, ICustomLogger<InMemoryCacheHelper> customLogger)
        {
            _memoryCache = memoryCache;
            _customLogger = customLogger;
        }

        public Task<T> GetAsync<T>(string key)
        {
            return Task.FromResult(_memoryCache.Get<T>(key));
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? timeSpan = null)
        {
            _memoryCache.Set(key, value);
            return Task.CompletedTask;
        }

        public Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> generator, TimeSpan? timeSpan = null)
        {
            return _memoryCache.GetOrCreateAsync(key, entry =>
            {
                _customLogger.Verbose($"Retrieving from cache {key}");
                entry.SlidingExpiration = timeSpan;
                return generator();
            });
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }
    }
}
