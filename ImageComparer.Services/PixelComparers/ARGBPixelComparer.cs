using System.Drawing;
using ImageDiff.CommonAbstractions;

namespace ImageDiff.Services.PixelComparers
{
    public class ARGBPixelComparer : IPixelComparer
    {
        public bool Equal(Color pixel1, Color pixel2)
        {
            return pixel1.ToArgb() == pixel2.ToArgb();
        }
    }
}
