using System;

namespace ImageDiff.CommonAbstractions
{
    public interface IStorage<TKey, TObject>
    {
        TObject Get(TKey objectKey);

        void Save(TKey objectKey, TObject resultImage, DateTime absoluteExpiration);
    }
}