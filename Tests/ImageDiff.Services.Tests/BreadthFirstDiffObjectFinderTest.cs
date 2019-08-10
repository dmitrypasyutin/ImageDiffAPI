using System.Drawing;
using System.Linq;
using ImageDiff.CommonAbstractions;
using ImageDiff.Data;
using ImageDiff.Services.DiffObjectFinders;
using Moq;
using NUnit.Framework;

namespace ImageDiff.Services.Tests
{
    public class BreadthFirstDiffObjectFinderTest
    {
        private Mock<IBitmapComparer> _imageComparerMock;
        private Bitmap _image1;
        private Bitmap _image2;
        private string _image1FileName = "image1.bmp";
        private string _image2FileName = "image2.bmp";

        private ImagePixel[] _diffPixels = { };

        [SetUp]
        public void Init()
        {
            _image1 = CreateBitmap(_image1FileName);
            _image2 = CreateBitmap(_image2FileName);

            _imageComparerMock = new Mock<IBitmapComparer>();

            _imageComparerMock.Setup(f => f.PixelsEqual(_image1, _image2,
                It.Is<ImagePixel>(ff=> _diffPixels.Contains(ff)))).Returns(false);

            _imageComparerMock.Setup(f => f.PixelsEqual(_image1, _image2,
                It.Is<ImagePixel>(ff => !_diffPixels.Contains(ff)))).Returns(true);
        }

        private Bitmap CreateBitmap(string fileName)
        {
            return Image.FromFile(fileName) as Bitmap;
        }

        private ImagePixel[] GetDiff1Object()
        {
            return new ImagePixel[]
            {
                new ImagePixel(1, 1),
                new ImagePixel(1, 2),
                new ImagePixel(1, 3)
            };
        }

        private ImagePixel[] GetDiff2Objects()
        {
            return new ImagePixel[]
            {
                new ImagePixel(1, 1),
                new ImagePixel(1, 2),
                new ImagePixel(3, 3),
                new ImagePixel(3, 4),
            };
        }

        [Test]
        public void FindAllDiffObjects_Finds_1_Object()
        {
            _diffPixels = GetDiff1Object();

            var finder = new BreadthFirstDiffObjectsFinder(_imageComparerMock.Object);
            var result = finder.FindAllDiffObjects(_image1, _image2).ToArray();

            Assert.AreEqual(result.Count(), 1);
            Assert.AreEqual(result[0].TopLeft, new ImagePixel(1, 1));
            Assert.AreEqual(result[0].BottomRight, new ImagePixel(1, 3));
        }

        [Test]
        public void FindAllDiffObjects_Finds_2_Objects()
        {
            _diffPixels = GetDiff2Objects();

            var finder = new BreadthFirstDiffObjectsFinder(_imageComparerMock.Object);
            var result = finder.FindAllDiffObjects(_image1, _image2).ToArray();

            Assert.AreEqual(result.Count(), 2);
            Assert.AreEqual(result[0].TopLeft, new ImagePixel(1, 1));
            Assert.AreEqual(result[0].BottomRight, new ImagePixel(1, 2));
            Assert.AreEqual(result[1].TopLeft, new ImagePixel(3, 3));
            Assert.AreEqual(result[1].BottomRight, new ImagePixel(3, 4));
        }

        [Test]
        public void FindAllDiffObjects_Finds_0_Objects()
        {
            var finder = new BreadthFirstDiffObjectsFinder(_imageComparerMock.Object);
            var result = finder.FindAllDiffObjects(_image1, _image2).ToArray();

            Assert.AreEqual(result.Count(), 0);
        }
    }
}