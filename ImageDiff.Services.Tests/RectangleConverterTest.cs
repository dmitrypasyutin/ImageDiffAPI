using ImageDiff.Data;
using ImageDiff.Services.Converters;
using NUnit.Framework;

namespace ImageDiff.Services.Tests
{
    public class RectangleConverterTest
    {
        [Test]
        public void ToRectangleF_Converts_Correctly()
        {
            var topLeft = new ImagePixel(1, 1);
            var bottomRight = new ImagePixel(3, 3);
            var rectangle = new Rectangle(topLeft, bottomRight);
            var rectangleF = rectangle.ToRectangleF();

            Assert.AreEqual(topLeft.X, rectangleF.X);
            Assert.AreEqual(topLeft.Y, rectangleF.Y);
            Assert.AreEqual(rectangleF.Width, bottomRight.X - topLeft.X);
            Assert.AreEqual(rectangleF.Height, bottomRight.Y - topLeft.Y);
        }
    }
}