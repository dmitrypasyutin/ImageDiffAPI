using NUnit.Framework;

namespace ImageDiff.Data.Tests
{
    public class RectangleTests
    {
        [Test]
        public void Test_Constructor()
        {
            var topLeftPixel = new ImagePixel(1, 1);
            var bottomRightPixel = new ImagePixel(4, 4);

            var rectangle = new Rectangle(topLeftPixel, bottomRightPixel);
            Assert.AreEqual(rectangle.TopLeft, topLeftPixel);
            Assert.AreEqual(rectangle.BottomRight, bottomRightPixel);
        }
    }
}