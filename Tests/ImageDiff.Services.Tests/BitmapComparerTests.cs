using System.Drawing;
using ImageDiff.CommonAbstractions;
using ImageDiff.Data;
using ImageDiff.Services.ImageComparers;
using Moq;
using NUnit.Framework;

namespace ImageDiff.Services.Tests
{
    public class BitmapComparerTests
    {
        private readonly Mock<IPixelComparer> _pixelComparerMock = new Mock<IPixelComparer>();
        private Bitmap _image1;
        private Bitmap _image2;
        private string _image1FileName = "image1.bmp";
        private string _image2FileName = "image2.bmp";

        [SetUp]
        public void Init()
        {
            _image1 = CreateBitmap(_image1FileName);
            _image2 = CreateBitmap(_image2FileName);

            _pixelComparerMock.Setup(f => f.Equal(It.IsAny<Color>(), It.IsAny<Color>()))
                .Returns<Color, Color>((a, b) => a.ToArgb() == b.ToArgb());
        }

        private Bitmap CreateBitmap(string fileName)
        {
            return Image.FromFile(fileName) as Bitmap;
        }

        [Test]
        public void Equal_True_When_Pixels_Same_Color()
        {
            var comparer = new BitmapComparer(_pixelComparerMock.Object);
            Assert.IsTrue(comparer.PixelsEqual(_image1, _image2, new ImagePixel(0, 0)));
            Assert.IsFalse(comparer.PixelsEqual(_image1, _image2, new ImagePixel(2, 2)));
        }
    }
}