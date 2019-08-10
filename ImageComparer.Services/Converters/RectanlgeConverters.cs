using System.Drawing;
using Rectangle = ImageDiff.Data.Rectangle;

namespace ImageDiff.Services.Converters
{
    public static class RectangleConverters
    {
        public static RectangleF ToRectangleF(this Rectangle rectangle)
        {
            return new RectangleF(rectangle.TopLeft.X, rectangle.TopLeft.Y, rectangle.BottomRight.X - rectangle.TopLeft.X,
                rectangle.BottomRight.Y - rectangle.TopLeft.Y);
        }
    }
}