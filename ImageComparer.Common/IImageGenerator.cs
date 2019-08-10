using Rectangle = ImageDiff.Data.Rectangle;

namespace ImageDiff.CommonAbstractions
{
    public interface IImageGenerator
    {
        byte[] DrawRectangles(byte[] imageData, string contentType, params Rectangle[] rectangles);
    }
}