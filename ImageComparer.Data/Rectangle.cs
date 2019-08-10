using System.Drawing;

namespace ImageDiff.Data
{
    public struct Rectangle
    {
        public readonly ImagePixel TopLeft;
        public readonly ImagePixel BottomRight;

        public Rectangle(ImagePixel topLeft, ImagePixel bottomRight)
        {
            TopLeft = topLeft;
            BottomRight = bottomRight;
        }
    }
}