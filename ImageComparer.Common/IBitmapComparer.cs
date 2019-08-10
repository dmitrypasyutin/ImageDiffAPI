using System.Drawing;
using ImageDiff.Data;

namespace ImageDiff.CommonAbstractions
{
    public interface IBitmapComparer
    {
        bool PixelsEqual(Bitmap image1, Bitmap image2, ImagePixel coordinate);
    }
}