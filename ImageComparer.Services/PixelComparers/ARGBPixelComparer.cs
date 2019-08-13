using System;
using System.Drawing;
using ImageDiff.CommonAbstractions;

namespace ImageDiff.Services.PixelComparers
{
    public class ARGBPixelComparer : IPixelComparer
    {
        private readonly int _tolerance;

        public ARGBPixelComparer(int tolerance)
        {
            _tolerance = tolerance;
        }
        
        
        public bool Equal(Color pixel1, Color pixel2)
        {
            return Math.Abs(pixel1.A - pixel2.A) <= _tolerance
                   && Math.Abs(pixel1.R - pixel2.R) <= _tolerance
                   && Math.Abs(pixel1.G - pixel2.G) <= _tolerance
                   && Math.Abs(pixel1.B - pixel2.B) <= _tolerance;
        }
    }
}
