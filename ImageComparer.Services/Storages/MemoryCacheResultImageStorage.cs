using System;
using ImageDiff.CommonAbstractions;
using ImageDiff.Data;

namespace ImageDiff.Services.Storages
{
    public class MemoryCacheResultImageStorage<TKey> : IResultImageStorage<TKey>
    {
        private readonly IStorage<TKey, ResultImage> _storage;

        public MemoryCacheResultImageStorage(IStorage<TKey, ResultImage> storage)
        {
            _storage = storage ?? throw new ArgumentException(nameof(storage));
        }

        public ResultImage Get(TKey imageStorageKey)
        {
            return _storage.Get(imageStorageKey);
        }

        public void Save(TKey imageStorageKey, ResultImage resultImage, DateTime absoluteExpiration)
        {
            _storage.Save(imageStorageKey, resultImage, absoluteExpiration);
        }

        public void UpdatePercentAndVersion(TKey imageStorageKey, int percentProcessed, int imageVersion)
        {
            var resultImage = Get(imageStorageKey);
            if (resultImage != null)
            {
                resultImage.PercentsProcessed = percentProcessed;
                resultImage.ImageVersion = imageVersion;
            }
        }
        
        public void UpdatePercent(TKey imageStorageKey, int percentProcessed)
        {
            var resultImage = Get(imageStorageKey);
            if (resultImage != null)
            {
                resultImage.PercentsProcessed = percentProcessed;
            }
        }
    }
}