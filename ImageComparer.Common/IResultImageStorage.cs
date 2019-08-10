using System;
using ImageDiff.Data;

namespace ImageDiff.CommonAbstractions
{
    public interface IResultImageStorage<TKey>
    {
        ResultImage Get(TKey imageStorageKey);
        void Save(TKey imageStorageKey, ResultImage resultImage, DateTime absoluteExpiration);
        void UpdatePercentAndVersion(TKey imageStorageKey, int newPercentProcessed, int newImageVersion);
        void UpdatePercent(TKey imageStorageKey, int newPercentProcessed);
    }
}