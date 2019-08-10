using System.Drawing;

namespace ImageDiff.CommonAbstractions
{
    public interface IPixelComparer
    {
        bool Equal(Color pixel1, Color pixel2);
    }
}
