using System.Collections.Generic;
using System.Drawing;
using Rectangle = ImageDiff.Data.Rectangle;

namespace ImageDiff.CommonAbstractions
{
    public interface IDiffObjectsFinder
    {
        IEnumerable<Rectangle> FindAllDiffObjects(Bitmap image1, Bitmap image2);
    }
}