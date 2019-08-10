using System;
using ImageDiff.CommonAbstractions;
using Microsoft.Extensions.Caching.Memory;

namespace ImageDiff.Core
{
    public class MemoryCacheStorage<TKey, TObject> : IStorage<TKey, TObject>
    {
        private readonly IMemoryCache _cache;

        public MemoryCacheStorage(IMemoryCache cache)
        {
            _cache = cache ?? throw new ArgumentException(nameof(cache));
        }

        public TObject Get(TKey objectKey)
        {
            return _cache.Get<TObject>(objectKey);
        }

        public void Save(TKey objectKey, TObject storeObject, DateTime absoluteExpiration)
        {
            _cache.Set(objectKey, storeObject, absoluteExpiration);
        }
    }
}