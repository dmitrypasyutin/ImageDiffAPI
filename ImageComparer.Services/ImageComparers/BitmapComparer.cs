using System;
using System.Drawing;
using ImageDiff.CommonAbstractions;
using ImageDiff.Data;

namespace ImageDiff.Services.ImageComparers
{
    public class BitmapComparer : IBitmapComparer
    {
        private readonly IPixelComparer _pixelComparer;

        public BitmapComparer(IPixelComparer pixelComparer)
        {
            _pixelComparer = pixelComparer;
        }

        public bool PixelsEqual(Bitmap image1, Bitmap image2, ImagePixel coordinate)
        {
            if(image1 is null) throw new ArgumentException(nameof(image1));
            if(image2 is null) throw new ArgumentException(nameof(image2));
            
            Color pixel1 = image1.GetPixel(coordinate.X, coordinate.Y);
            Color pixel2 = image2.GetPixel(coordinate.X, coordinate.Y);
            return _pixelComparer.Equal(pixel1, pixel2);
        }
    }
}