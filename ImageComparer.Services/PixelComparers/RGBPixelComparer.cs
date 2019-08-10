using System.Drawing;
using ImageDiff.CommonAbstractions;

namespace ImageDiff.Services.PixelComparers
{
    public class RGBPixelsComparer : IPixelComparer
    {
        public bool Equal(Color pixel1, Color pixel2)
        {
            return pixel1.R == pixel2.R && pixel1.G == pixel2.G && pixel1.B == pixel2.B;
        }
    }
}